using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        
        public ReviewsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetReviews()
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.Status == true)
                .ToList();
                
            return Ok(reviews);
        }

        [HttpGet("ByUser/{userId}")]
        [Produces("application/json")]
        public IActionResult GetReviewsByUserID(string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Geçersiz kullanıcı ID formatı.");
            }

            var reviews = _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserID == userIdInt && r.Status == true)
                .ToList();

            if (reviews == null || !reviews.Any())
            {
                // NotFound yerine boş liste döndürmek daha uygun olabilir
                // return NotFound();
            }

            return Ok(reviews);
        }

        [HttpGet("ByProduct/{productId}")]
        [Produces("application/json")]
        public IActionResult GetReviewsByProductID(int productId)
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductID == productId && r.Status == true)
                .ToList();

            if (reviews == null || !reviews.Any())
            {
                return NotFound();
            }

            return Ok(reviews);
        }

        [HttpGet("ByStore/{storeId}")]
        [Produces("application/json")]
        public IActionResult GetReviewsByStoreID(int storeId)
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.Product != null && r.Product.StoreID == storeId && r.Status == true)
                .ToList();

            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewsDTO reviewsDTO)
        {
            if (reviewsDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı veya geçersiz." });
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == reviewsDTO.ProductID && p.Status == true);
            if (product == null)
            {
                return BadRequest(new { Message = "Ürün bulunamadı." });
            }

            if (reviewsDTO.OrderItemID.HasValue)
            {
                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .FirstOrDefaultAsync(oi => oi.OrderItemID == reviewsDTO.OrderItemID.Value &&
                                               oi.ProductID == reviewsDTO.ProductID &&
                                               oi.Order.UserID == userId);

                if (orderItem == null)
                {
                    return BadRequest(new { Message = "Bu ürünü belirttiğiniz sipariş detayı ile satın almamışsınız." });
                }

                if (orderItem.Order.Status != "Pending" && orderItem.Order.Status != "Delivered")
                {
                    return BadRequest(new { Message = $"Sipariş durumu '{orderItem.Order.Status}' olduğu için bu ürüne henüz yorum yapamazsınız." });
                }

                bool hasReviewForOrderItem = await _context.Reviews
                    .AnyAsync(r => r.UserID == userId && r.OrderItemID == reviewsDTO.OrderItemID.Value && r.Status == true);

                if (hasReviewForOrderItem)
                {
                    return BadRequest(new { Message = "Bu siparişinizdeki ürün için zaten bir inceleme yapmışsınız." });
                }
            }
            else
            {
                bool hasReviewForProductGenerally = await _context.Reviews
                    .AnyAsync(r => r.UserID == userId && r.ProductID == reviewsDTO.ProductID && r.OrderItemID == null && r.Status == true);
                if (hasReviewForProductGenerally)
                {
                     return BadRequest(new { Message = "Bu ürün için daha önce genel bir yorum yapmışsınız. Sipariş üzerinden yorum yapmayı deneyin."});
                }
            }

            var review = _mapper.Map<Reviews>(reviewsDTO);
            review.UserID = userId;
            review.OrderItemID = reviewsDTO.OrderItemID;
            review.Status = true;
            review.ReviewDate = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var createdReviewDetails = await _context.Reviews
                .Where(r => r.ReviewID == review.ReviewID)
                .Select(r => new ReviewsResponseDTO
                {
                    ReviewID = r.ReviewID,
                    UserID = r.UserID,
                    ProductID = r.ProductID,
                    OrderItemID = r.OrderItemID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate,
                    UserFullName = r.User != null ? (r.User.FirstName + " " + r.User.LastName) : "Bilinmeyen Kullanıcı",
                    ProductName = r.Product != null ? r.Product.ProductName : "Bilinmeyen Ürün",
                    ProductImageUrl = r.Product != null ? r.Product.ImageUrl : null,
                    UserAvatarUrl = null
                })
                .FirstOrDefaultAsync();

            return Ok(createdReviewDetails);
        }

        [HttpPut("{id}")]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewsDTO reviewsDTO)
        {
            if (reviewsDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı veya geçersiz." });
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewID == id && r.Status == true);
            if (review == null)
            {
                return NotFound(new { Message = "Güncellenecek yorum bulunamadı." });
            }

            if (review.UserID != currentUserId)
            {
                return Forbid("Bu yorumu güncelleme yetkiniz yok.");
            }

            if (review.ProductID != reviewsDTO.ProductID)
            {
                return BadRequest(new { Message = "Yorumun ait olduğu ürün değiştirilemez." });
            }

            if (review.OrderItemID != reviewsDTO.OrderItemID)
            {
                 return BadRequest(new { Message = "Yorumun ait olduğu sipariş kalemi değiştirilemez." });
            }

            review.Rating = reviewsDTO.Rating;
            review.Comment = reviewsDTO.Comment;
            review.ReviewDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updatedReviewDetails = await _context.Reviews
                .Where(r => r.ReviewID == review.ReviewID)
                .Select(r => new ReviewsResponseDTO {
                    ReviewID = r.ReviewID,
                    UserID = r.UserID,
                    ProductID = r.ProductID,
                    OrderItemID = r.OrderItemID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate,
                    UserFullName = r.User != null ? (r.User.FirstName + " " + r.User.LastName) : "Bilinmeyen Kullanıcı",
                    ProductName = r.Product != null ? r.Product.ProductName : "Bilinmeyen Ürün",
                    ProductImageUrl = r.Product != null ? r.Product.ImageUrl : null,
                    UserAvatarUrl = null
                })
                .FirstOrDefaultAsync();
            
            return Ok(updatedReviewDetails);
        }

        [HttpGet("me/reviewable-order-items")]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> GetReviewableOrderItems()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı veya geçersiz."});
            }

            var reviewableItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.UserID == currentUserId &&
                             (oi.Order.Status == "Pending" || oi.Order.Status == "Delivered") &&
                             !_context.Reviews.Any(r => r.OrderItemID == oi.OrderItemID && r.UserID == currentUserId && r.Status == true))
                .Select(oi => new ReviewableProductDto
                {
                    OrderItemId = oi.OrderItemID,
                    OrderId = oi.OrderID,
                    ProductId = oi.ProductID,
                    ProductName = oi.Product.ProductName,
                    ProductImageUrl = oi.Product.ImageUrl,
                    OrderDate = oi.Order.OrderDate,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                })
                .OrderByDescending(oi => oi.OrderDate)
                .ToListAsync();

            return Ok(reviewableItems);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int currentUserId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği alınamadı veya geçersiz." });
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewID == id);
            if (review == null)
            {
                return NotFound(new { Message = "Silinecek yorum bulunamadı." });
            }

            if (review.UserID != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid("Bu yorumu silme yetkiniz yok.");
            }

            review.Status = false;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Yorum başarıyla silindi." });
        }

        [HttpGet("details/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetReviewDetailsById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.ReviewID == id && r.Status == true)
                .Select(r => new ReviewsResponseDTO
                {
                    ReviewID = r.ReviewID,
                    UserID = r.UserID,
                    ProductID = r.ProductID,
                    OrderItemID = r.OrderItemID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate,
                    UserFullName = r.User != null ? (r.User.FirstName + " " + r.User.LastName) : "Bilinmeyen Kullanıcı",
                    UserAvatarUrl = null,
                    ProductName = r.Product != null ? r.Product.ProductName : "Bilinmeyen Ürün",
                    ProductImageUrl = r.Product != null ? r.Product.ImageUrl : null
                })
                .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound(new { Message = "Yorum bulunamadı." });
            }

            return Ok(review);
        }
    }
} 