using AutoMapper;
using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Temel yol: api/FavoriteLists
    [Authorize] 
    public class FavoriteListsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<FavoriteListsController> _logger;

        public FavoriteListsController(KTUN_DbContext context, IMapper mapper, ILogger<FavoriteListsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out var userId))
            {
                return userId;
            }
            _logger.LogWarning("GetCurrentUserId - User ID claim not found or invalid.");
            return null;
        }
        private bool IsAdmin()
        {
            return User.IsInRole("Admin"); // Rol adınız farklıysa güncelleyin
        }

        // --- FAVORİ LİSTELERİ İÇİN ENDPOINTLER (GET, POST, PUT, DELETE sırasıyla) ---

        // GET api/FavoriteLists/users/{userId}
        [HttpGet("users/{userId}")] 
        public async Task<ActionResult<IEnumerable<FavoriteListResponseDTO>>> GetUserFavoriteLists(int userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("User ID not found in token.");

            IQueryable<FavoriteList> query = _context.FavoriteLists.Where(fl => fl.UserID == userId && fl.Status);

            if (userId != currentUserId.Value && !IsAdmin())
            {
                // İstek yapan kişi ne listenin sahibi ne de admin ise sadece public listeleri görebilir.
                query = query.Where(fl => !fl.IsPrivate);
            }
            // Admin ise veya kendi listelerini istiyorsa tüm listelerini (private dahil) görebilir.
            
            var lists = await query.Include(fl => fl.User).OrderByDescending(fl => fl.CreatedAt).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<FavoriteListResponseDTO>>(lists));
        }

        // GET api/FavoriteLists/public
        [HttpGet("public")]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<FavoriteListResponseDTO>>> GetAllPublicFavoriteLists()
        {
            _logger.LogInformation("GetAllPublicFavoriteLists - Fetching all public and active favorite lists.");
            var publicLists = await _context.FavoriteLists
                                        .Where(fl => !fl.IsPrivate && fl.Status)
                                        .Include(fl => fl.User) 
                                        .OrderByDescending(fl => fl.CreatedAt) 
                                        .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<FavoriteListResponseDTO>>(publicLists));
        }

        // GET api/FavoriteLists/{listId} (Liste üst bilgileri, öğeler hariç)
        [HttpGet("{listId}")]
        public async Task<ActionResult<FavoriteListResponseDTO>> GetFavoriteListById(int listId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var favoriteList = await _context.FavoriteLists
                                         .Include(fl => fl.User) 
                                         .FirstOrDefaultAsync(fl => fl.FavoriteListID == listId && fl.Status);

            if (favoriteList == null) 
            {
                _logger.LogWarning("GetFavoriteListById - Active favorite list with ID {ListId} not found.", listId);
                return NotFound("Active favorite list not found.");
            }

            if (favoriteList.IsPrivate && favoriteList.UserID != currentUserId.Value && !IsAdmin())
            {
                _logger.LogWarning("GetFavoriteListById - User {UserId} (not owner/admin) attempted to access private list {ListId}.", currentUserId, listId);
                return Forbid("This list is private and you are not the owner or an admin.");
            }
            return Ok(_mapper.Map<FavoriteListResponseDTO>(favoriteList));
        }

        // POST api/FavoriteLists/users/{userId}
        [HttpPost("users/{userId}")]
        public async Task<ActionResult<FavoriteListResponseDTO>> CreateFavoriteListForUser(int userId, [FromBody] FavoriteListDTO favoriteListDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null || (currentUserId.Value != userId && !IsAdmin())) // Sadece admin başkası adına liste oluşturabilir
            {
                _logger.LogWarning("CreateFavoriteListForUser - Unauthorized attempt to create list for user {TargetUserId} by user {ActualUserId}.", userId, currentUserId);
                return Unauthorized("Cannot create a favorite list for another user unless you are an admin.");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var favoriteList = _mapper.Map<FavoriteList>(favoriteListDto);
            favoriteList.UserID = userId; // Admin başkası adına oluşturuyorsa userId parametresini kullanır
            favoriteList.CreatedAt = DateTime.UtcNow;
            favoriteList.Status = true; 

            _context.FavoriteLists.Add(favoriteList);
            await _context.SaveChangesAsync();
            _logger.LogInformation("CreateFavoriteListForUser - New favorite list created with ID {FavoriteListID} for user {UserID}.", favoriteList.FavoriteListID, userId);

            await _context.Entry(favoriteList).Reference(fl => fl.User).LoadAsync();
            return CreatedAtAction(nameof(GetFavoriteListById), new { listId = favoriteList.FavoriteListID }, _mapper.Map<FavoriteListResponseDTO>(favoriteList));
        }

        // PUT api/FavoriteLists/{listId}
        [HttpPut("{listId}")]
        public async Task<IActionResult> UpdateFavoriteList(int listId, [FromBody] FavoriteListDTO favoriteListDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var listToUpdate = await _context.FavoriteLists.FirstOrDefaultAsync(fl => fl.FavoriteListID == listId && fl.Status);

            if (listToUpdate == null)
            {
                _logger.LogWarning("UpdateFavoriteList - Active favorite list with ID {ListId} not found.", listId);
                return NotFound("Active favorite list not found.");
            }

            if (listToUpdate.UserID != currentUserId.Value && !IsAdmin()) // Sadece sahibi veya admin güncelleyebilir
            {
                _logger.LogWarning("UpdateFavoriteList - User {UserId} (not owner/admin) attempted to update list {ListId}.", currentUserId, listId);
                return Forbid("You can only update your own favorite lists or you are not an admin.");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _mapper.Map(favoriteListDto, listToUpdate); 
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("UpdateFavoriteList - Favorite list {ListId} updated by user {UserId}.", listId, currentUserId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "UpdateFavoriteList - Concurrency exception while updating list {ListId}.", listId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating favorite list.");
            }
            return NoContent(); 
        }

        // DELETE api/FavoriteLists/{listId}
        [HttpDelete("{listId}")]
        public async Task<IActionResult> DeleteFavoriteList(int listId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var listToDelete = await _context.FavoriteLists.FindAsync(listId); // Status fark etmez, silinecek listeyi bul

            if (listToDelete == null)
            {
                _logger.LogWarning("DeleteFavoriteList - Favorite list with ID {ListId} not found for deletion.", listId);
                return NotFound("Favorite list not found.");
            }

            if (listToDelete.UserID != currentUserId.Value && !IsAdmin()) // Sadece sahibi veya admin silebilir
            {
                 _logger.LogWarning("DeleteFavoriteList - User {UserId} (not owner/admin) attempted to delete list {ListId}.", currentUserId, listId);
                return Forbid("You can only delete your own favorite lists or you are not an admin.");
            }
            
            listToDelete.Status = false; // Soft delete
            await _context.SaveChangesAsync();
            _logger.LogInformation("DeleteFavoriteList - Favorite list {ListId} (soft) deleted by user {UserId}.", listId, currentUserId);

            return NoContent(); 
        }

        // --- FAVORİ LİSTESİ ÖĞELERİ (ÜRÜNLER) İÇİN ENDPOINTLER (GET, POST, DELETE sırasıyla) ---

        // GET api/FavoriteLists/{listId}/products
        [HttpGet("{listId}/products")] 
        public async Task<ActionResult<IEnumerable<FavoriteListItemResponseDTO>>> GetFavoriteListItems(int listId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var favoriteList = await _context.FavoriteLists.FirstOrDefaultAsync(fl => fl.FavoriteListID == listId && fl.Status);
            if (favoriteList == null)
            {
                _logger.LogWarning("GetFavoriteListItems - Active favorite list with ID {ListId} not found.", listId);
                return NotFound("Active favorite list not found.");
            }

            if (favoriteList.IsPrivate && favoriteList.UserID != currentUserId.Value && !IsAdmin())
            {
                _logger.LogWarning("GetFavoriteListItems - User {UserId} (not owner/admin) attempted to access items of private list {ListId}.", currentUserId, listId);
                return Forbid("This list is private and you are not the owner or an admin.");
            }

            var items = await _context.FavoriteListItems
                                .Where(fli => fli.FavoriteListID == listId)
                                .Include(fli => fli.Product) 
                                .OrderBy(fli => fli.AddedDate) // Öğeleri eklenme tarihine göre sırala
                                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<FavoriteListItemResponseDTO>>(items));
        }

        // POST api/FavoriteLists/{listId}/products
        [HttpPost("{listId}/products")]
        public async Task<ActionResult<FavoriteListItemResponseDTO>> AddProductToFavoriteList(int listId, [FromBody] FavoriteListItemDTO listItemDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var favoriteList = await _context.FavoriteLists.FirstOrDefaultAsync(fl => fl.FavoriteListID == listId && fl.Status);
            if (favoriteList == null)
            {
                _logger.LogWarning("AddProductToFavoriteList - Active favorite list with ID {ListId} not found to add product.", listId);
                return NotFound("Active favorite list not found to add product.");
            }

            if (favoriteList.UserID != currentUserId.Value && !IsAdmin()) // Sadece sahibi veya admin listeye ürün ekleyebilir
            {
                _logger.LogWarning("AddProductToFavoriteList - User {UserId} (not owner/admin) attempted to add product to list {ListId}.", currentUserId, listId);
                return Forbid("You can only add products to your own favorite lists or you are not an admin.");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var productExists = await _context.Products.AnyAsync(p => p.ProductID == listItemDto.ProductId && p.Status);
            if (!productExists)
            {
                _logger.LogWarning("AddProductToFavoriteList - Active product with ID {ProductId} not found.", listItemDto.ProductId);
                return NotFound("Active product not found.");
            }

            var itemExists = await _context.FavoriteListItems
                                       .AnyAsync(fli => fli.FavoriteListID == listId && fli.ProductID == listItemDto.ProductId);
            if (itemExists)
            {
                return Conflict("Product already exists in this favorite list.");
            }

            var favoriteListItem = _mapper.Map<FavoriteListItem>(listItemDto);
            favoriteListItem.FavoriteListID = listId;
            favoriteListItem.AddedDate = DateTime.UtcNow;

            _context.FavoriteListItems.Add(favoriteListItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("AddProductToFavoriteList - Product {ProductId} added to list {ListId} by user {UserId}.", listItemDto.ProductId, listId, currentUserId);

            await _context.Entry(favoriteListItem).Reference(fli => fli.Product).LoadAsync();
            return CreatedAtAction(nameof(GetFavoriteListItems), new { listId = listId }, _mapper.Map<FavoriteListItemResponseDTO>(favoriteListItem));
        }

        // DELETE api/FavoriteLists/{listId}/products/{productId}
        [HttpDelete("{listId}/products/{productId}")]
        public async Task<IActionResult> RemoveProductFromFavoriteList(int listId, int productId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var favoriteList = await _context.FavoriteLists.FirstOrDefaultAsync(fl => fl.FavoriteListID == listId && fl.Status);
            if (favoriteList == null) // Silme işlemi için listenin aktif olması gerekmez, ama kontrol ediyoruz.
            {
                _logger.LogWarning("RemoveProductFromFavoriteList - Favorite list with ID {ListId} not found.", listId);
                return NotFound("Favorite list not found.");
            }

            if (favoriteList.UserID != currentUserId.Value && !IsAdmin()) // Sadece sahibi veya admin listeden ürün silebilir
            {
                _logger.LogWarning("RemoveProductFromFavoriteList - User {UserId} (not owner/admin) attempted to remove product from list {ListId}.", currentUserId, listId);
                return Forbid("You can only remove products from your own favorite lists or you are not an admin.");
            }

            var itemToRemove = await _context.FavoriteListItems
                                           .FirstOrDefaultAsync(fli => fli.FavoriteListID == listId && fli.ProductID == productId);
            
            if (itemToRemove == null)
            {
                _logger.LogWarning("RemoveProductFromFavoriteList - Product {ProductId} not found in list {ListId}.", productId, listId);
                return NotFound("Product not found in this favorite list.");
            }

            _context.FavoriteListItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            _logger.LogInformation("RemoveProductFromFavoriteList - Product {ProductId} removed from list {ListId} by user {UserId}.", productId, listId, currentUserId);

            return NoContent(); 
        }
    }
} 