using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using KTUN_Final_Year_Project.Services;
using Azure.Core;
using System.Diagnostics; // Stopwatch için eklendi

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
            var stopwatch = Stopwatch.StartNew(); // Zaman ölçümü başlat
            _logger.LogInformation("START GetImageCache - PageID: {PageID}, Prompt: {Prompt}", pageID, prompt);

            // CORS Headers (Bunlar genellikle middleware ile yönetilir, ama burada bırakıyorum)
            // Response.Headers.Append("Access-Control-Allow-Origin", "*");
            // Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            // Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Accept");

            if (string.IsNullOrEmpty(pageID) || string.IsNullOrEmpty(prompt))
            {
                _logger.LogWarning("GetImageCache - Bad Request: Missing required parameters.");
                return BadRequest("Missing required parameters");
            }

            string hashInput = prompt;
            string hashValue = GenerateHash(hashInput);
            _logger.LogInformation("GetImageCache - Generated HashValue: {HashValue} for Prompt: {Prompt}", hashValue, prompt);

            try
            {
                // 1. Redis'ten kontrol et
                _logger.LogInformation("GetImageCache - Checking Redis for HashValue: {HashValue}", hashValue);
                var redisStopwatch = Stopwatch.StartNew();
                var cachedImage = await _redisService.GetImageFromCacheAsync(hashValue);
                redisStopwatch.Stop();
                _logger.LogInformation("GetImageCache - Redis check completed in {ElapsedMilliseconds}ms. Found: {Found}", redisStopwatch.ElapsedMilliseconds, !string.IsNullOrEmpty(cachedImage));

                if (!string.IsNullOrEmpty(cachedImage))
                {
                    stopwatch.Stop();
                    _logger.LogInformation("END GetImageCache - Found in Redis. Returning OK. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                    return Ok(new { success = true, image = cachedImage, source = "redis_cache" }); // 'success' ve 'source' eklendi
                }

                // 2. Redis'te yoksa veritabanından al
                _logger.LogInformation("GetImageCache - Checking Database for PageID: {PageID}, HashValue: {HashValue}", pageID, hashValue);
                var dbStopwatch = Stopwatch.StartNew();
                var imageCacheEntity = await _context.ImageCache.FirstOrDefaultAsync(ic =>
                    ic.PageID == pageID && ic.HashValue == hashValue);
                dbStopwatch.Stop();
                _logger.LogInformation("GetImageCache - Database check completed in {ElapsedMilliseconds}ms. Found: {Found}", dbStopwatch.ElapsedMilliseconds, imageCacheEntity != null);

                if (imageCacheEntity == null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("END GetImageCache - Not Found in DB or Redis. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                    return NotFound(new { success = false, message = "Image not found in cache." }); // 'success' eklendi
                }

                if (imageCacheEntity.Image == null || imageCacheEntity.Image.Length == 0)
                {
                    _logger.LogWarning("GetImageCache - Found record in DB (ID: {ID}) but Image data is empty.", imageCacheEntity.ID);
                    // Burada image üretme tetiklenmeli mi? Eğer evetse, o kod burada olmalı.
                    // Şimdilik 'NotFound' dönüyorum, çünkü resim yok.
                    stopwatch.Stop();
                    _logger.LogWarning("END GetImageCache - Found in DB but image empty. Returning NotFound. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                    return NotFound(new { success = false, message = "Image record found but data is empty." });
                }

                var base64Image = Convert.ToBase64String(imageCacheEntity.Image);
                _logger.LogInformation("GetImageCache - Image found in DB (ID: {ID}). Image size: {ImageSize} bytes.", imageCacheEntity.ID, imageCacheEntity.Image.Length);

                // 3. Veritabanından alınan veriyi Redis'e kaydet (arka planda?)
                _logger.LogInformation("GetImageCache - Saving DB image to Redis for HashValue: {HashValue}", hashValue);
                var redisSaveStopwatch = Stopwatch.StartNew();
                // Bu işlemi await kullanmadan arka planda yapmak isteyebilirsiniz (Fire-and-forget)
                // Ancak loglama için await ile devam ediyorum.
                await _redisService.SaveImageToCacheAsync(hashValue, base64Image);
                redisSaveStopwatch.Stop();
                _logger.LogInformation("GetImageCache - Saved image to Redis in {ElapsedMilliseconds}ms.", redisSaveStopwatch.ElapsedMilliseconds);

                stopwatch.Stop();
                _logger.LogInformation("END GetImageCache - Found in DB, saved to Redis. Returning OK. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, image = base64Image, source = "database_cache" }); // 'success' ve 'source' eklendi
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageCache - Exception occurred after {ElapsedMilliseconds}ms. PageID: {PageID}, Prompt: {Prompt}", stopwatch.ElapsedMilliseconds, pageID, prompt);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" }); // 'success' eklendi
            }
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateImageCache([FromBody] ImageCacheResponseDTO imageCacheResponseDTO)
        {
            var stopwatch = Stopwatch.StartNew();
            // Gelen isteği detaylı logla
            _logger.LogInformation("START CreateImageCache - Received DTO: {@ImageCacheDTO}", imageCacheResponseDTO);

            try
            {
                if (imageCacheResponseDTO == null || string.IsNullOrEmpty(imageCacheResponseDTO.PageID) || string.IsNullOrEmpty(imageCacheResponseDTO.Prompt))
                {
                    _logger.LogWarning("CreateImageCache - Bad Request: DTO is null or missing PageID/Prompt.");
                    stopwatch.Stop(); // Hata durumunda da süreyi loglamak iyi olabilir
                    return BadRequest(new { success = false, message = "Request body is invalid or missing required fields (PageID, Prompt)."});
                }

                // !!! EN ÖNEMLİ KONTROL: Resim verisi geldi mi?
                if (string.IsNullOrEmpty(imageCacheResponseDTO.Base64Image))
                {
                    _logger.LogError("CreateImageCache - Bad Request: Base64Image field is missing or empty in the received POST request for Prompt: {Prompt}", imageCacheResponseDTO.Prompt);
                    // Bu durum olmamalı, çünkü route.ts resmi göndermeli. Eğer buraya düşüyorsa, route.ts'in gönderdiği veri hatalı veya binding sorunu var.
                    stopwatch.Stop();
                    return BadRequest(new { success = false, message = "Base64Image data is required in the POST request body." });
                }

                byte[] imageBytes;
                try
                {
                    // Potansiyel olarak çok büyük string, dikkatli olalım.
                    imageBytes = Convert.FromBase64String(imageCacheResponseDTO.Base64Image);
                    _logger.LogInformation("CreateImageCache - Base64 decoding successful. Image size: {ImageSize} bytes for Prompt: {Prompt}", imageBytes.Length, imageCacheResponseDTO.Prompt);
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "CreateImageCache - Base64 decoding error for Prompt: {Prompt}", imageCacheResponseDTO.Prompt);
                    stopwatch.Stop();
                    return BadRequest(new { success = false, message = "Invalid Base64 image data." });
                }

                if (imageBytes.Length == 0)
                {
                    _logger.LogWarning("CreateImageCache - Decoded image data is empty for Prompt: {Prompt}", imageCacheResponseDTO.Prompt);
                    stopwatch.Stop();
                    return BadRequest(new { success = false, message = "Provided image data is empty after decoding." });
                }


                var hashValue = GenerateHash(imageCacheResponseDTO.Prompt);
                _logger.LogInformation("CreateImageCache - Generated HashValue: {HashValue} for Prompt: {Prompt}", hashValue, imageCacheResponseDTO.Prompt);

                // Veritabanı kontrolü
                _logger.LogInformation("CreateImageCache - Checking Database for existing record. PageID: {PageID}, HashValue: {HashValue}", imageCacheResponseDTO.PageID, hashValue);
                var dbStopwatch = Stopwatch.StartNew();
                var existingRecord = await _context.ImageCache
                    .FirstOrDefaultAsync(ic => ic.PageID == imageCacheResponseDTO.PageID && ic.HashValue == hashValue);
                dbStopwatch.Stop();
                _logger.LogInformation("CreateImageCache - Database check completed in {ElapsedMilliseconds}ms. Found: {Found}", dbStopwatch.ElapsedMilliseconds, existingRecord != null);


                if (existingRecord != null)
                {
                    // Mevcut kaydı GÜNCELLE
                    _logger.LogInformation("CreateImageCache - Existing DB record found (ID: {ID}). Updating with new image (Size: {ImageSize} bytes).", existingRecord.ID, imageBytes.Length);
                    existingRecord.Image = imageBytes;
                    existingRecord.Status = true;
                    existingRecord.Prompt = imageCacheResponseDTO.Prompt; // Prompt'u da güncellemek iyi olabilir

                    try
                    {
                        var saveDbStopwatch = Stopwatch.StartNew();
                        await _context.SaveChangesAsync(); // <-- Hata yakalama önemli
                        saveDbStopwatch.Stop();
                        _logger.LogInformation("CreateImageCache - DB record updated successfully in {ElapsedMilliseconds}ms.", saveDbStopwatch.ElapsedMilliseconds);

                        // Redis'i de güncelle
                        string newBase64Image = imageCacheResponseDTO.Base64Image; // Zaten string olarak var
                        _logger.LogInformation("CreateImageCache - Updating Redis cache for HashValue: {HashValue}", hashValue);
                        var redisUpdateStopwatch = Stopwatch.StartNew();
                        await _redisService.SaveImageToCacheAsync(hashValue, newBase64Image);
                        redisUpdateStopwatch.Stop();
                        _logger.LogInformation("CreateImageCache - Redis cache updated in {ElapsedMilliseconds}ms.", redisUpdateStopwatch.ElapsedMilliseconds);

                        stopwatch.Stop();
                        _logger.LogInformation("END CreateImageCache - Record updated in DB and Redis. Returning OK. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                        return Ok(new { success = true, image = newBase64Image, message = "Image cache record updated successfully.", source = "update" });
                    }
                    catch (DbUpdateException dbEx)
                    {
                        stopwatch.Stop();
                        _logger.LogError(dbEx, "ERROR CreateImageCache (DbUpdateException) - Failed to update existing DB record (ID: {ID}) after {ElapsedMilliseconds}ms. Inner Exception: {InnerException}",
                            existingRecord.ID, stopwatch.ElapsedMilliseconds, dbEx.InnerException?.Message);
                        return StatusCode(500, new { success = false, message = $"Database error updating record: {dbEx.Message} | Inner: {dbEx.InnerException?.Message}" });
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _logger.LogError(ex, "ERROR CreateImageCache (General Exception) - Failed to update existing DB record (ID: {ID}) after {ElapsedMilliseconds}ms.",
                            existingRecord.ID, stopwatch.ElapsedMilliseconds);
                        return StatusCode(500, new { success = false, message = $"Internal server error updating record: {ex.Message}" });
                    }
                }
                else
                {
                    // YENİ kayıt oluştur
                    _logger.LogInformation("CreateImageCache - No existing record found. Creating new record with image size: {ImageSize} bytes.", imageBytes.Length);
                    var newImageCache = new ImageCache
                    {
                        PageID = imageCacheResponseDTO.PageID,
                        Prompt = imageCacheResponseDTO.Prompt,
                        Image = imageBytes,
                        HashValue = hashValue,
                        Status = true,
                        // CreatedDate alanı ImageCache entity'sinde var mı kontrol edilmeli,
                        // Eğer varsa ve otomatik setlenmiyorsa: CreatedDate = DateTime.UtcNow
                    };
                    _context.ImageCache.Add(newImageCache);

                    try
                    {
                        var saveDbStopwatch = Stopwatch.StartNew();
                        await _context.SaveChangesAsync(); // <-- Hata yakalama önemli
                        saveDbStopwatch.Stop();
                        _logger.LogInformation("CreateImageCache - New DB record created successfully (ID: {ID}) in {ElapsedMilliseconds}ms.", newImageCache.ID, saveDbStopwatch.ElapsedMilliseconds);

                        // Redis'e kaydet
                        string newBase64Image = imageCacheResponseDTO.Base64Image;
                        _logger.LogInformation("CreateImageCache - Saving new image to Redis for HashValue: {HashValue}", hashValue);
                        var redisSaveStopwatch = Stopwatch.StartNew();
                        await _redisService.SaveImageToCacheAsync(hashValue, newBase64Image);
                        redisSaveStopwatch.Stop();
                        _logger.LogInformation("CreateImageCache - Saved new image to Redis in {ElapsedMilliseconds}ms.", redisSaveStopwatch.ElapsedMilliseconds);

                        stopwatch.Stop();
                        _logger.LogInformation("END CreateImageCache - New record created in DB and Redis. Returning OK. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                        return Ok(new { success = true, image = newBase64Image, message = "Image cache record created successfully.", source = "new_entry" });
                    }
                    catch (DbUpdateException dbEx)
                    {
                        stopwatch.Stop();
                        _logger.LogError(dbEx, "ERROR CreateImageCache (DbUpdateException) - Failed to create new DB record after {ElapsedMilliseconds}ms. Inner Exception: {InnerException}",
                            stopwatch.ElapsedMilliseconds, dbEx.InnerException?.Message);
                        return StatusCode(500, new { success = false, message = $"Database error creating record: {dbEx.Message} | Inner: {dbEx.InnerException?.Message}" });
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _logger.LogError(ex, "ERROR CreateImageCache (General Exception) - Failed to create new DB record after {ElapsedMilliseconds}ms.",
                           stopwatch.ElapsedMilliseconds);
                        return StatusCode(500, new { success = false, message = $"Internal server error creating record: {ex.Message}" });
                    }
                }
            }
            catch (Exception ex) // Genel try-catch bloğu
            {
                stopwatch.Stop();
                // DTO loglaması en başta yapıldığı için burada tekrar loglamaya gerek yok belki ama hata durumunda tekrar görmek faydalı olabilir.
                _logger.LogError(ex, "ERROR CreateImageCache - Outer Exception occurred after {ElapsedMilliseconds}ms. Request Data: {@Request}", stopwatch.ElapsedMilliseconds, imageCacheResponseDTO);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}", details = ex.StackTrace });
            }
        }


        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> SoftDeleteImageCacheByStatus(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("START SoftDeleteImageCacheByStatus - ID: {ID}", id);

            var imageCache = await _context.ImageCache.FindAsync(id);

            if (imageCache == null)
            {
                _logger.LogWarning("SoftDeleteImageCacheByStatus - Not Found: Record with ID {ID} not found.", id);
                stopwatch.Stop();
                return NotFound(new { success = false, message = $"Image cache record with ID {id} not found." });
            }

            _logger.LogInformation("SoftDeleteImageCacheByStatus - Found record (ID: {ID}). Current Status: {Status}, HashValue: {HashValue}", imageCache.ID, imageCache.Status, imageCache.HashValue);

            // Redis'ten sil (varsa)
            if (!string.IsNullOrEmpty(imageCache.HashValue))
            {
                _logger.LogInformation("SoftDeleteImageCacheByStatus - Deleting from Redis cache. HashValue: {HashValue}", imageCache.HashValue);
                var redisDeleteStopwatch = Stopwatch.StartNew();
                await _redisService.DeleteFromCacheAsync(imageCache.HashValue);
                redisDeleteStopwatch.Stop();
                _logger.LogInformation("SoftDeleteImageCacheByStatus - Redis delete completed in {ElapsedMilliseconds}ms.", redisDeleteStopwatch.ElapsedMilliseconds);
            }

            // DB'de durumu güncelle (Soft delete)
            imageCache.Status = false;
            _logger.LogInformation("SoftDeleteImageCacheByStatus - Updating DB record status to false for ID: {ID}", id);
            var dbUpdateStopwatch = Stopwatch.StartNew();
            await _context.SaveChangesAsync();
            dbUpdateStopwatch.Stop();
            _logger.LogInformation("SoftDeleteImageCacheByStatus - DB status updated in {ElapsedMilliseconds}ms.", dbUpdateStopwatch.ElapsedMilliseconds);


            stopwatch.Stop();
            _logger.LogInformation("END SoftDeleteImageCacheByStatus - Completed successfully. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            // NoContent() yerine OK dönmek, frontend'e bilgi vermek için daha iyi olabilir.
            return Ok(new { success = true, message = $"Image cache record with ID {id} marked as deleted." });
            // return NoContent(); // Orijinal yanıt
        }

        // Hash üretme fonksiyonu (private ve static olabilir)
        private static string GenerateHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                // _logger.LogWarning("GenerateHash - Input string is null or empty."); // Static metodda logger yok
                return string.Empty;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                // StringBuilder kullanmak daha performanslı olabilir ama kısa string için fark etmez.
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant(); // ToLowerInvariant daha güvenli
            }
        }
    }
}
