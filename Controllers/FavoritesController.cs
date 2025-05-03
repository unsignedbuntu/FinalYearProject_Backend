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
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var favorites = await _context.UserFavorites
                .Where(f => f.UserID == userId)
                .Include(f => f.Product) // Ensure Product data is loaded
                .Select(f => new FavoriteDto
                {
                    ProductId = f.ProductID,
                    ProductName = f.Product.ProductName,
                    Price = f.Product.Price,
                    AddedDate = f.AddedDate
                })
                .ToListAsync();

            return Ok(favorites);
        }

        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequestDto request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // Check if product exists
            var productExists = await _context.Products.AnyAsync(p => p.ProductID == request.ProductId);
            if (!productExists)
            {
                return BadRequest("Product not found.");
            }

            // Check if the favorite already exists for this user
            var favoriteExists = await _context.UserFavorites
                .AnyAsync(f => f.UserID == userId && f.ProductID == request.ProductId);

            if (favoriteExists)
            {
                // Already exists, return Conflict or Ok depending on desired behavior
                return Conflict("Product is already in favorites.");
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
                // Log the exception details (ex)
                Console.WriteLine($"Error adding favorite: {ex.ToString()}"); // Use the 'ex' variable
                // Could be a unique constraint violation if checked again due to race condition
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the favorite.");
            }

            // Optionally return the created favorite details
            // For simplicity, returning Ok or CreatedAtAction
            return Ok(); // Or CreatedAtAction(nameof(GetUserFavorites), null)
        }

        // DELETE: api/Favorites/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User ID not found in token.");
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
            catch (Exception ex) // Consider logging the exception details: ex.ToString()
            {
                // Log exception (consider a logging framework)
                Console.WriteLine($"Error removing favorite: {ex.ToString()}"); // Log full exception details
                return StatusCode(500, "Favori kaldırılırken bir sunucu hatası oluştu.");
            }

            return NoContent(); // Successfully removed
        }

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