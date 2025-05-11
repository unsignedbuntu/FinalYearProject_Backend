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
    [Authorize] // Require authentication for all actions in this controller
    public class FavoritesController : ControllerBase
    {
        private readonly KTUN_DbContext _context;

        public FavoritesController(KTUN_DbContext context)
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

        // GET: api/Favorites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetUserFavorites()
        {
            if (!TryGetUserId(out var userId) || userId == 0) // userId == 0 kontrolü eklendi
            {
                return Unauthorized("User ID not found in token or invalid.");
            }

            var favorites = await _context.UserFavorites
                .Where(uf => uf.UserID == userId) // .Value kaldırıldı çünkü userId artık int
                .Include(uf => uf.Product)
                .Select(uf => new FavoriteDto
                {
                    ProductId = uf.ProductID,
                    ProductName = uf.Product.ProductName,
                    Price = uf.Product.Price,
                    ImageUrl = uf.Product.ImageUrl,
                    InStock = uf.Product.StockQuantity > 0,
                    AddedDate = uf.AddedDate
                })
                .ToListAsync();

            return Ok(favorites);
        }

        // Helper method GetUserIdFromClaims kaldırıldı, TryGetUserId yeterli.

        // POST: api/Favorites
        [HttpPost]
        public async Task<ActionResult<FavoriteDto>> AddFavorite([FromBody] AddFavoriteRequestDto request) // IActionResult yerine ActionResult<FavoriteDto>
        {
            if (!TryGetUserId(out var userId) || userId == 0)
            {
                return Unauthorized("User ID not found in token or invalid.");
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductID == request.ProductId);
            if (!productExists)
            {
                return BadRequest("Product not found.");
            }

            var favoriteExists = await _context.UserFavorites
                .AnyAsync(f => f.UserID == userId && f.ProductID == request.ProductId);

            if (favoriteExists)
            {
                // Mevcut favoriyi DTO olarak döndür
                var existingFavoriteDto = await _context.UserFavorites
                    .Where(f => f.UserID == userId && f.ProductID == request.ProductId)
                    .Include(f => f.Product)
                    .Select(uf => new FavoriteDto
                    {
                        ProductId = uf.ProductID,
                        ProductName = uf.Product.ProductName,
                        Price = uf.Product.Price,
                        ImageUrl = uf.Product.ImageUrl,
                        InStock = uf.Product.StockQuantity > 0,
                        AddedDate = uf.AddedDate
                    })
                    .FirstOrDefaultAsync();

                // Eğer bir şekilde DTO null gelirse (beklenmedik durum), genel bir conflict mesajı dönebilirsin.
                // Normalde `existingFavoriteDto` null olmamalı çünkü `favoriteExists` true.
                return Conflict(existingFavoriteDto ?? (object)"Product is already in favorites.");
            }

            var newFavorite = new UserFavorite
            {
                UserID = userId,
                ProductID = request.ProductId,
                AddedDate = DateTime.UtcNow
            };

            _context.UserFavorites.Add(newFavorite);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error adding favorite: {ex.ToString()}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the favorite.");
            }

            // Yeni eklenen favorinin detaylarını çek ve DTO olarak döndür
            var createdFavoriteDto = await _context.UserFavorites
                .Where(f => f.UserFavoriteID == newFavorite.UserFavoriteID) // Yeni eklenen kaydın ID'si ile bul
                .Include(f => f.Product)
                .Select(uf => new FavoriteDto
                {
                    ProductId = uf.ProductID,
                    ProductName = uf.Product.ProductName,
                    Price = uf.Product.Price,
                    ImageUrl = uf.Product.ImageUrl,
                    InStock = uf.Product.StockQuantity > 0,
                    AddedDate = uf.AddedDate
                    // Örnek olarak inStock veya supplierName gibi ek alanlar eklenebilir:
                    // inStock = uf.Product.StockQuantity > 0,
                    // supplierName = uf.Product.Suppliers.FirstOrDefault()?.SupplierName
                })
                .FirstOrDefaultAsync();

            if (createdFavoriteDto == null)
            {
                // Bu durum normalde SaveChangesAsync başarılı olduysa ve newFavorite.UserFavoriteID geçerliyse yaşanmamalı.
                Console.WriteLine($"Critical error: Could not retrieve favorite DTO for UserFavoriteID {newFavorite.UserFavoriteID} after creation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not retrieve favorite details after creation.");
            }

            // Oluşturulan DTO'yu 200 OK (veya 201 Created) ile döndür
            return Ok(createdFavoriteDto);
            // Alternatif: return CreatedAtAction(nameof(GetUserFavorites), null, createdFavoriteDto); // Bu 201 Created döndürür
        }

        // DELETE: api/Favorites/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            if (!TryGetUserId(out var userId) || userId == 0)
            {
                return Unauthorized("User ID not found in token or invalid.");
            }

            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.ProductID == productId);

            if (favorite == null)
            {
                return NotFound("Favorite not found.");
            }

            _context.UserFavorites.Remove(favorite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing favorite: {ex.ToString()}");
                return StatusCode(500, "Favori kaldırılırken bir sunucu hatası oluştu.");
            }

            return NoContent(); // Successfully removed
        }

        // GetUserId metodu TryGetUserId ile birleştirildiği için kaldırıldı.
    }
}