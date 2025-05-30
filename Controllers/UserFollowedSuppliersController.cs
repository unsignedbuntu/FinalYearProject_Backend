using AutoMapper;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/users/{userId}/followed-suppliers")]
    [ApiController]
    [Authorize] // Tüm endpointler için yetkilendirme gerektir
    public class UserFollowedSuppliersController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;

        public UserFollowedSuppliersController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private bool IsUserAuthorized(int userIdFromRoute, out int currentUserId)
        {
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdString, out currentUserId))
            {
                return false; // Token'da UserID yoksa veya geçersizse yetkisiz
            }
            return userIdFromRoute == currentUserId; // Route'daki userId ile token'daki userId eşleşmeli
        }

        // GET: api/users/{userId}/followed-suppliers
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<FollowedSupplierResponseDto>>>> GetFollowedSuppliers(int userId)
        {
            if (!IsUserAuthorized(userId, out _))
            {
                return Unauthorized(new ApiResponseDto<List<FollowedSupplierResponseDto>> { Success = false, Message = "Unauthorized to access this resource." });
            }

            var followedSuppliers = await _context.UserFollowedSuppliers
                .Where(ufs => ufs.UserID == userId)
                .Include(ufs => ufs.Supplier) // Supplier bilgilerini çekmek için
                .OrderByDescending(ufs => ufs.FollowedDate) // En son takip edilenler üste gelsin
                .ToListAsync();

            if (!followedSuppliers.Any())
            {
                return Ok(new ApiResponseDto<List<FollowedSupplierResponseDto>> { Success = true, Data = new List<FollowedSupplierResponseDto>(), Message = "No suppliers followed yet." });
            }

            var responseDto = _mapper.Map<List<FollowedSupplierResponseDto>>(followedSuppliers);
            return Ok(new ApiResponseDto<List<FollowedSupplierResponseDto>> { Success = true, Data = responseDto });
        }

        // POST: api/users/{userId}/followed-suppliers/{supplierId}
        [HttpPost("{supplierId}")]
        public async Task<ActionResult<ApiResponseDto<object>>> FollowSupplier(int userId, int supplierId)
        {
            if (!IsUserAuthorized(userId, out var currentUserId))
            {
                return Unauthorized(new ApiResponseDto<object> { Success = false, Message = "Unauthorized to perform this action." });
            }

            // Tedarikçinin var olup olmadığını kontrol et
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierID == supplierId && s.Status == true);
            if (!supplierExists)
            {
                return NotFound(new ApiResponseDto<object> { Success = false, Message = "Supplier not found or is not active." });
            }

            // Kullanıcının bu tedarikçiyi zaten takip edip etmediğini kontrol et
            var alreadyFollowing = await _context.UserFollowedSuppliers
                .AnyAsync(ufs => ufs.UserID == currentUserId && ufs.SupplierID == supplierId);

            if (alreadyFollowing)
            {
                // Zaten takip ediyorsa, bir şey yapma veya mevcut takip bilgisini döndür. Şimdilik sadece OK.
                return Ok(new ApiResponseDto<object> { Success = true, Message = "Supplier is already followed." });
            }

            var newFollow = new UserFollowedSupplier
            {
                UserID = currentUserId,
                SupplierID = supplierId,
                FollowedDate = DateTime.UtcNow
            };

            _context.UserFollowedSuppliers.Add(newFollow);
            await _context.SaveChangesAsync();

            // Başarı durumunda takip edilen tedarikçinin detaylarını veya sadece başarı mesajını dönebiliriz.
            // Örnek olarak ID'sini ve mesajı dönelim.
            return Ok(new ApiResponseDto<object> 
            { 
                Success = true, 
                Message = "Supplier followed successfully.",
                Data = new { userFollowedSupplierId = newFollow.UserFollowedSupplierID } // Opsiyonel: Oluşan kaydın ID'si
            });
        }

        // DELETE: api/users/{userId}/followed-suppliers/{supplierId}
        [HttpDelete("{supplierId}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UnfollowSupplier(int userId, int supplierId)
        {
            if (!IsUserAuthorized(userId, out var currentUserId))
            {
                return Unauthorized(new ApiResponseDto<object> { Success = false, Message = "Unauthorized to perform this action." });
            }

            var followEntry = await _context.UserFollowedSuppliers
                .FirstOrDefaultAsync(ufs => ufs.UserID == currentUserId && ufs.SupplierID == supplierId);

            if (followEntry == null)
            {
                return NotFound(new ApiResponseDto<object> { Success = false, Message = "Follow relationship not found." });
            }

            _context.UserFollowedSuppliers.Remove(followEntry);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<object> { Success = true, Message = "Supplier unfollowed successfully." });
        }
    }
} 