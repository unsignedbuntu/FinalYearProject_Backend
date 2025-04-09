using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using KTUN_Final_Year_Project.Services;
using Azure.Core;


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

                var base64Image = Convert.ToBase64String(imageCacheEntity.Image ?? Array.Empty<byte>());

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

                _logger.LogInformation($"Received Request: PageID={imageCacheResponseDTO.PageID}, Prompt={imageCacheResponseDTO.Prompt}, ImageProvided={!string.IsNullOrEmpty(imageCacheResponseDTO.Base64Image)}");

                byte[] imageBytes;
                if (string.IsNullOrEmpty(imageCacheResponseDTO.Base64Image))
                {
                    _logger.LogWarning("Base64Image field is NULL or empty. Cannot proceed without image data.");
                    imageBytes = Array.Empty<byte>();
                }
                else
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(imageCacheResponseDTO.Base64Image);
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogError($"Base64 decoding error: {ex.Message}");
                        return BadRequest("Invalid Base64 image data.");
                    }
                }

                var hashValue = GenerateHash(imageCacheResponseDTO.Prompt ?? string.Empty);

                var existingRecord = await _context.ImageCache
                    .FirstOrDefaultAsync(ic => ic.PageID == imageCacheResponseDTO.PageID && ic.HashValue == hashValue);

                if (existingRecord != null)
                {
                    _logger.LogInformation($"Existing record found with ID: {existingRecord.ID}");

                    if (existingRecord.Image == null || existingRecord.Image.Length == 0)
                    {
                        _logger.LogInformation("Existing record has no image, updating with new image");
                        existingRecord.Image = imageBytes;
                        existingRecord.Status = true;
                        await _context.SaveChangesAsync();

                        return Ok(new
                        {
                            success = true,
                            image = Convert.ToBase64String(existingRecord.Image ?? Array.Empty<byte>()),
                            message = "Kayıt güncellendi"
                        });
                    }
                    else
                    {
                        _logger.LogInformation("Existing record has image, returning conflict with existing image");
                        return Conflict(new
                        {
                            success = true,
                            image = Convert.ToBase64String(existingRecord.Image),
                            message = "Bu kayıt zaten mevcut!"
                        });
                    }
                }

                _logger.LogInformation("Creating new record");
                var imageCacheEntity = new ImageCache
                {
                    PageID = imageCacheResponseDTO.PageID,
                    Prompt = imageCacheResponseDTO.Prompt,
                    HashValue = hashValue,
                    Image = imageBytes,
                    Status = true
                };

                _context.ImageCache.Add(imageCacheEntity);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    image = Convert.ToBase64String(imageCacheEntity.Image ?? Array.Empty<byte>()),
                    message = "Yeni kayıt oluşturuldu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata oluştu: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    details = ex.StackTrace
                });
            }
        }


        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> SoftDeleteImageCacheByStatus(int id)
        {
            var imageCache = await _context.ImageCache.FindAsync(id);

            if (imageCache == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(imageCache.HashValue))
            {
                await _redisService.DeleteFromCacheAsync(imageCache.HashValue);
            }

            imageCache.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
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
