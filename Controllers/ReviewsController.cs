using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetReviewByID(int id)
        {
            var review = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefault(r => r.ReviewID == id && r.Status == true);

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
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
        [Produces("application/json")]
        public IActionResult CreateReview([FromBody] ReviewsDTO reviewsDTO)
        {
            if (reviewsDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kullanıcı varlığını kontrol et
            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(reviewsDTO.UserID));
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı");
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductID == reviewsDTO.ProductID && p.Status == true);
            if (product == null)
            {
                return BadRequest("Ürün bulunamadı");
            }

            // Kullanıcının ürünü satın alıp almadığını kontrol et (opsiyonel)
            bool hasPurchased = _context.Orders
                .Where(o => o.UserID == int.Parse(reviewsDTO.UserID) && o.Status == "Active")
                .Join(_context.OrderItems.Where(oi => oi.ProductID == reviewsDTO.ProductID),
                      order => order.OrderID,
                      orderItem => orderItem.OrderID,
                      (order, orderItem) => order)
                .Any();

            if (!hasPurchased)
            {
                return BadRequest("Bu ürünü satın almadığınız için inceleme yapamazsınız");
            }

            // Kullanıcının bu ürün için zaten bir incelemesi var mı kontrol et
            bool hasReview = _context.Reviews
                .Where(r => r.UserID == int.Parse(reviewsDTO.UserID) && r.ProductID == reviewsDTO.ProductID && r.Status == true)
                .Any();

            if (hasReview)
            {
                return BadRequest("Bu ürün için zaten bir inceleme yapmışsınız");
            }

            var review = _mapper.Map<Reviews>(reviewsDTO);
            review.Status = true;
            review.ReviewDate = DateTime.Now;

            _context.Reviews.Add(review);
            _context.SaveChanges();

            // Ürünün ortalama puanını güncelle
            UpdateProductAverageRating(reviewsDTO.ProductID);

            var createdReview = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefault(r => r.ReviewID == review.ReviewID);

            return Ok(createdReview);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateReview(int id, [FromBody] ReviewsDTO reviewsDTO)
        {
            if (reviewsDTO == null)
            {
                return BadRequest();
            }

            var review = _context.Reviews.FirstOrDefault(r => r.ReviewID == id && r.Status == true);
            if (review == null)
            {
                return NotFound();
            }

            // İncelemenin sahibi olduğunu kontrol et
            if (review.UserID != int.Parse(reviewsDTO.UserID))
            {
                return Forbid();
            }

            // İncelemeyi güncelle
            review.Rating = reviewsDTO.Rating;
            review.Comment = reviewsDTO.Comment;
            review.ReviewDate = DateTime.Now;

            _context.Reviews.Update(review);
            _context.SaveChanges();

            // Ürünün ortalama puanını güncelle
            UpdateProductAverageRating(reviewsDTO.ProductID);

            var updatedReview = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefault(r => r.ReviewID == id);

            return Ok(updatedReview);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteReviewByStatus(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.ReviewID == id);
            if (review == null)
            {
                return NotFound();
            }

            int productID = review.ProductID;

            review.Status = false;
            _context.Reviews.Update(review);
            _context.SaveChanges();

            // Ürünün ortalama puanını güncelle
            UpdateProductAverageRating(productID);

            return NoContent();
        }
        
        private void UpdateProductAverageRating(int productID)
        {
            // Products entity'sinden AverageRating kaldırıldığı için bu metodun işlevi kalmadı.
            // Gerekirse, ürün puanları ayrı bir şekilde hesaplanıp saklanabilir veya dinamik olarak çekilebilir.
            /*
            var product = _context.Products.FirstOrDefault(p => p.ProductID == productID);
            if (product != null)
            {
                var ratings = _context.Reviews
                    .Where(r => r.ProductID == productID && r.Status == true)
                    .Select(r => r.Rating)
                    .ToList();

                product.AverageRating = ratings.Any() ? (decimal)ratings.Average() : 0;
                _context.Products.Update(product);
                _context.SaveChanges();
            }
            */
        }
    }
} 