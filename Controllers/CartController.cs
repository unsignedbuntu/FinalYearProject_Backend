using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class CartController : ControllerBase
    {
        private readonly KTUN_DbContext _context;

        public CartController(KTUN_DbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out userId))
            {
                return true;
            }
            userId = 0;
            return false;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetUserCartItems()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var cartItems = await _context.UserCartItems
                .Where(c => c.UserID == userId)
                .Include(c => c.Product) // Ensure Product data is loaded
                .Select(c => new CartItemDto // Project to CartItemDto
                {
                    UserCartItemId = c.UserCartItemID,
                    ProductId = c.ProductID,
                    ProductName = c.Product.ProductName,
                    Quantity = c.Quantity,
                    Price = c.Product.Price,
                    // ImageUrl is commented out in DTO
                    AddedDate = c.AddedDate
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        // POST: api/Cart
        // Adds a new item or updates the quantity of an existing item
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> AddOrUpdateCartItem([FromBody] AddOrUpdateCartItemRequestDto request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            // Check if product exists
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return BadRequest("Product not found.");
            }

            // Check if item already exists in the cart for this user
            var existingCartItem = await _context.UserCartItems
                .FirstOrDefaultAsync(ci => ci.UserID == userId && ci.ProductID == request.ProductId);

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity = request.Quantity;
                existingCartItem.AddedDate = DateTime.UtcNow; // Update timestamp maybe?
                _context.UserCartItems.Update(existingCartItem);
            }
            else
            {
                // Add new cart item
                existingCartItem = new UserCartItem
                {
                    UserID = userId,
                    ProductID = request.ProductId,
                    Quantity = request.Quantity,
                    AddedDate = DateTime.UtcNow
                };
                _context.UserCartItems.Add(existingCartItem);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log exception (ex)
                Console.WriteLine($"Error adding/updating cart item: {ex.ToString()}"); // Log full exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the cart.");
            }

            // Return the updated/added item DTO
            var resultDto = new CartItemDto
            {
                UserCartItemId = existingCartItem.UserCartItemID,
                ProductId = existingCartItem.ProductID,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = existingCartItem.Quantity,
                AddedDate = existingCartItem.AddedDate
            };

            return Ok(resultDto);
        }

        // DELETE: api/Cart/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveCartItem(int productId)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var cartItem = await _context.UserCartItems
                .FirstOrDefaultAsync(ci => ci.UserID == userId && ci.ProductID == productId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            _context.UserCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent(); // Successfully removed
        }

        // DELETE: api/Cart (Optional: Clear entire cart) - REMOVING DUPLICATE
        /*
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var cartItems = await _context.UserCartItems
                .Where(ci => ci.UserID == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return Ok("Cart is already empty."); // Or NoContent()
            }

            _context.UserCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return NoContent(); // Successfully cleared
        }
        */

        // Sepeti temizle
        [HttpDelete("clear")]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Kullanıcı girişi yapılmamış." });
            }

            try
            {
                var cartItems = await _context.UserCartItems
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.UserCartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Sepet başarıyla temizlendi." });
            }
            catch (Exception ex) // Consider logging the exception details: ex.ToString()
            {
                // Log the exception
                Console.WriteLine($"Error clearing cart: {ex.ToString()}"); // Log full exception details
                return StatusCode(500, new { message = "Sepet temizlenirken bir hata oluştu." });
            }
        }

        // Sepet içeriğini görüntüle (Daha önce tanımlandıysa birleştirilebilir veya bu kaldırılabilir)
        // GET api/cart endpoint'i zaten yukarıda var, bu muhtemelen gereksiz.
        // Eğer farklı bir amaç taşıyorsa, route'u değiştirilmeli veya silinmeli.
        // Bu GetCartItems metodu yukarıdaki GetCart metodu ile aynı işi yapıyor gibi duruyor.
        // Eğer farklı bir detay seviyesi veya filtreleme istenmiyorsa birini kaldırabiliriz.
        // Şimdilik yoruma alıyorum, gerekirse aktif edilebilir.
        /*
        [HttpGet("items")] // Örnek farklı bir route
        [Authorize]
        public async Task<IActionResult> GetCartItems() // Method ismi de farklı olabilir
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var cartItems = await _context.UserCartItems
                                          .Where(ci => ci.UserID == userId)
                                          .Include(ci => ci.Product) // Include Product details
                                          .Select(ci => new {
                                              ci.UserCartItemID,
                                              ci.ProductID,
                                              ProductName = ci.Product.ProductName, // Corrected Property
                                              ci.Quantity,
                                              ci.Product.Price,
                                              // ImageUrl = ci.Product.ImageUrl // Corrected Property (Assuming it exists now or commenting out)
                                          })
                                          .ToListAsync();

            return Ok(cartItems);
        }
        */

        // Helper method to get User ID from claims
        private int GetUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out var userId))
            {
                return userId;
            }
            return 0;
        }
    }
} 