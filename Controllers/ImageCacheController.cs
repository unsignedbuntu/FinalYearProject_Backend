using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using KTUN_Final_Year_Project.Services;


namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageCacheController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private readonly ILogger<ImageCacheController> _logger;

        public ImageCacheController(KTUN_DbContext context, IMapper mapper, IRedisService redisService, ILogger<ImageCacheController> logger)
        {
            _context = context;
            _mapper = mapper;
            _redisService = redisService;
            _logger = logger;
        }

        [HttpGet("{pageID}/{prompt}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageCache(string pageID, string prompt)
        {
            _logger.LogInformation($"GetImageFromCache called with pageID: {pageID}, prompt: {prompt}");

            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Accept");

            if (string.IsNullOrEmpty(pageID) || string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Missing required parameters");
            }

            string hashInput = prompt;
            string hashValue = GenerateHash(hashInput);

            try
            {
                // Önce Redis'ten kontrol et
                var cachedImage = await _redisService.GetImageFromCacheAsync(hashValue);
                if (!string.IsNullOrEmpty(cachedImage))
                {
                    return Ok(new
                    {
                        cached = true,
                        image = cachedImage
                    });
                }

                // Redis'te yoksa veritabanından al
                var imageCacheEntity = _context.ImageCache.FirstOrDefault(ic =>
                    ic.PageID == pageID && ic.HashValue == hashValue);

                if (imageCacheEntity == null)
                {
                    return NotFound();
                }

                var base64Image = Convert.ToBase64String(imageCacheEntity.Image);

                // Veritabanından alınan veriyi Redis'e kaydet
                await _redisService.SaveImageToCacheAsync(hashValue, base64Image);

                return Ok(new
                {
                    cached = true,
                    image = base64Image
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateImageCache([FromBody] ImageCacheResponseDTO imageCacheResponseDTO)
        {
            try
            {
                if (imageCacheResponseDTO == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid request data");
                }

                string hashInput = imageCacheResponseDTO.Prompt;
                string hashValue = GenerateHash(hashInput);

                // Önce Redis'te kontrol et
                var cachedImage = await _redisService.GetImageFromCacheAsync(hashValue);
                if (!string.IsNullOrEmpty(cachedImage))
                {
                    return Ok(new
                    {
                        success = true,
                        image = cachedImage
                    });
                }

                // Redis'te yoksa veritabanında kontrol et
                var existingImageCache = _context.ImageCache.FirstOrDefault(ic =>
                    ic.PageID == imageCacheResponseDTO.PageID &&
                    ic.HashValue == hashValue);

                if (existingImageCache != null)
                {
                    var base64Image = Convert.ToBase64String(existingImageCache.Image);
                    // Veritabanından bulunan veriyi Redis'e kaydet
                    await _redisService.SaveImageToCacheAsync(hashValue, base64Image);
                    return Ok(new
                    {
                        success = true,
                        image = base64Image
                    });
                }

                // Yeni kayıt oluştur
                var imageCacheEntity = new ImageCache
                {
                    PageID = imageCacheResponseDTO.PageID,
                    Prompt = imageCacheResponseDTO.Prompt,
                    Image = imageCacheResponseDTO.Image,
                    HashValue = hashValue
                };

                _context.ImageCache.Add(imageCacheEntity);
                await _context.SaveChangesAsync();

                var newBase64Image = Convert.ToBase64String(imageCacheEntity.Image);
                // Yeni oluşturulan veriyi Redis'e kaydet
                await _redisService.SaveImageToCacheAsync(hashValue, newBase64Image);

                return Ok(new
                {
                    success = true,
                    image = newBase64Image
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> SoftDeleteImageCacheByStatus(int id)
        {
            var imageCache = _context.ImageCache.FirstOrDefault(ic => ic.ID == id);

            if (imageCache == null)
            {
                return NotFound();
            }

            // Redis'ten de sil
            await _redisService.DeleteFromCacheAsync(imageCache.HashValue);

            imageCache.Status = false;
            await _context.SaveChangesAsync();

            return Ok(imageCache);
        }

        private string GenerateHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
