using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using KTUN_Final_Year_Project.Services;
using System.Diagnostics;
using KTUN_Final_Year_Project.DTOs; // ImageCacheDTO için eklendi
using System.Linq; // .Select() için eklendi

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

        // Helper method to generate hash
        private static string GenerateHash(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        // GET: api/imagecache/image/{id} - Resmi ID ile direkt sunar
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("START GetImageById - ID: {ID}", id);

            try
            {
                var imageCacheEntity = await _context.ImageCache.FindAsync(id);

                if (imageCacheEntity == null || imageCacheEntity.Image == null || imageCacheEntity.Image.Length == 0)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("END GetImageById - Not Found or image data empty for ID: {ID}. Total time: {ElapsedMilliseconds}ms", id, stopwatch.ElapsedMilliseconds);
                    return NotFound(new { success = false, message = "Image not found or data is empty." });
                }

                stopwatch.Stop();
                _logger.LogInformation("END GetImageById - Found image for ID: {ID}. Returning File. Total time: {ElapsedMilliseconds}ms", id, stopwatch.ElapsedMilliseconds);
                return File(imageCacheEntity.Image, "image/png"); // Veya uygun MIME türü
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageById - Exception for ID: {ID}. Total time: {ElapsedMilliseconds}ms", id, stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" });
            }
        }


        // GET: api/imagecache/prompt/{prompt} - Prompt ile cache'den resim getirir (Redis -> DB)
        [HttpGet("prompt/{prompt}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageByPrompt(string prompt)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("START GetImageByPrompt - Prompt: {Prompt}", prompt);

            if (string.IsNullOrEmpty(prompt))
            {
                _logger.LogWarning("GetImageByPrompt - Bad Request: Missing prompt.");
                return BadRequest(new { success = false, message = "Prompt is required." });
            }

            string hashValue = GenerateHash(prompt);
            _logger.LogInformation("GetImageByPrompt - Generated HashValue: {HashValue} for Prompt: {Prompt}", hashValue, prompt);

            try
            {
                _logger.LogInformation("GetImageByPrompt - Checking Redis for HashValue: {HashValue}", hashValue);
                var redisStopwatch = Stopwatch.StartNew();
                var cachedImageBase64 = await _redisService.GetImageFromCacheAsync(hashValue);
                redisStopwatch.Stop();
                _logger.LogInformation("GetImageByPrompt - Redis check completed in {ElapsedMilliseconds}ms. Found: {Found}", redisStopwatch.ElapsedMilliseconds, !string.IsNullOrEmpty(cachedImageBase64));

                if (!string.IsNullOrEmpty(cachedImageBase64))
                {
                    stopwatch.Stop();
                    _logger.LogInformation("END GetImageByPrompt - Found in Redis. Returning OK. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                    // Response DTO'ya mapleyip Product/Supplier bilgilerini ekleyebiliriz, ama burada sadece resmi dönüyoruz.
                    // Eğer detaylı bilgi isteniyorsa, ImageCache entity'sini de çekip maplemeliyiz.
                    // Şimdilik sadece resim ve kaynak bilgisi dönüyor.
                    var dbEntityForInfo = await _context.ImageCache
                        .Include(ic => ic.Product)
                        .Include(ic => ic.Supplier)
                        .FirstOrDefaultAsync(ic => ic.HashValue == hashValue && ic.Status == true);

                    var response = new ImageCacheResponseDTO
                    {
                        ID = dbEntityForInfo?.ID ?? 0,
                        Prompt = prompt,
                        HashValue = hashValue,
                        Status = dbEntityForInfo?.Status ?? false,
                        ProductId = dbEntityForInfo?.ProductID,
                        ProductName = dbEntityForInfo?.Product?.ProductName,
                        SupplierId = dbEntityForInfo?.SupplierID,
                        SupplierName = dbEntityForInfo?.Supplier?.SupplierName,
                        Base64Image = cachedImageBase64,
                        ImageUrl = dbEntityForInfo != null ? $"/api/imagecache/image/{dbEntityForInfo.ID}" : null
                    };
                    return Ok(new { success = true, data = response, source = "redis_cache" });
                }

                _logger.LogInformation("GetImageByPrompt - Checking Database for HashValue: {HashValue}", hashValue);
                var dbStopwatch = Stopwatch.StartNew();
                var imageCacheEntity = await _context.ImageCache
                    .Include(ic => ic.Product)
                    .Include(ic => ic.Supplier)
                    .FirstOrDefaultAsync(ic => ic.HashValue == hashValue && ic.Status == true);
                dbStopwatch.Stop();
                _logger.LogInformation("GetImageByPrompt - Database check completed in {ElapsedMilliseconds}ms. Found: {Found}", dbStopwatch.ElapsedMilliseconds, imageCacheEntity != null);

                if (imageCacheEntity == null)
                {
                    stopwatch.Stop();
                    _logger.LogWarning("END GetImageByPrompt - Not Found in DB or Redis for Prompt: {Prompt}. Total time: {ElapsedMilliseconds}ms", prompt, stopwatch.ElapsedMilliseconds);
                    return NotFound(new { success = false, message = "Image not found for the given prompt." });
                }

                if (imageCacheEntity.Image == null || imageCacheEntity.Image.Length == 0)
                {
                    _logger.LogWarning("GetImageByPrompt - Found record in DB (ID: {ID}) but Image data is empty for Prompt: {Prompt}", imageCacheEntity.ID, prompt);
                    stopwatch.Stop();
                    return NotFound(new { success = false, message = "Image record found but data is empty." });
                }

                var base64Image = Convert.ToBase64String(imageCacheEntity.Image);
                _logger.LogInformation("GetImageByPrompt - Image found in DB (ID: {ID}). Image size: {ImageSize} bytes.", imageCacheEntity.ID, imageCacheEntity.Image.Length);

                _logger.LogInformation("GetImageByPrompt - Saving DB image to Redis for HashValue: {HashValue}", hashValue);
                var redisSaveStopwatch = Stopwatch.StartNew();
                await _redisService.SaveImageToCacheAsync(hashValue, base64Image);
                redisSaveStopwatch.Stop();
                _logger.LogInformation("GetImageByPrompt - Saved image to Redis in {ElapsedMilliseconds}ms.", redisSaveStopwatch.ElapsedMilliseconds);
                
                var mappedResponse = _mapper.Map<ImageCacheResponseDTO>(imageCacheEntity);
                // Eğer recordToSave'de Product/Supplier bilgisi yoksa (yeni eklenmişse ve Product/Supplier yüklenmemişse),
                // DTO'dan gelen ProductId/SupplierId ile tekrar yükleyip mapleyebiliriz veya mapper'a bırakabiliriz.
                // Mevcut mapper Product ve Supplier navigation property'lerinden isimleri alıyor.
                // SaveChanges sonrası recordToSave.Product ve recordToSave.Supplier null olabilir eğer ProductID/SupplierID yeni atandıysa ve explicit load yapılmadıysa.
                // Bu yüzden, response için gerekirse tekrar Product/Supplier yükleyelim.
                if (imageCacheEntity.ProductID.HasValue && imageCacheEntity.Product == null)
                {
                    imageCacheEntity.Product = await _context.Products.FindAsync(imageCacheEntity.ProductID.Value);
                    mappedResponse.ProductName = imageCacheEntity.Product?.ProductName;
                }
                 if (imageCacheEntity.SupplierID.HasValue && imageCacheEntity.Supplier == null)
                {
                    imageCacheEntity.Supplier = await _context.Suppliers.FindAsync(imageCacheEntity.SupplierID.Value);
                    mappedResponse.SupplierName = imageCacheEntity.Supplier?.SupplierName;
                }
                mappedResponse.Base64Image = base64Image; // İstemciye gönderilen resim
                mappedResponse.ImageUrl = $"/api/imagecache/image/{imageCacheEntity.ID}"; // Her zaman ImageUrl sağla

                stopwatch.Stop();
                _logger.LogInformation("END GetImageByPrompt - Found in DB, saved to Redis. Returning OK. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, data = mappedResponse, source = "database_cache" });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageByPrompt - Exception for Prompt: {Prompt}. Total time: {ElapsedMilliseconds}ms", prompt, stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        // POST: api/imagecache - Genel resim cache'i oluşturma/güncelleme (YENİ DTO ile)
        [HttpPost] // Route attribute'u class seviyesinde olduğu için path belirtmeye gerek yok ("/")
        [Produces("application/json")]
        public async Task<IActionResult> CreateOrUpdateImageCache([FromBody] ImageCacheDTO imageCacheDTO) // DTO Tipi Değişti
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("START CreateOrUpdateImageCache - Received DTO: {@ImageCacheDTO}", imageCacheDTO);

            if (imageCacheDTO == null || string.IsNullOrEmpty(imageCacheDTO.Prompt) || string.IsNullOrEmpty(imageCacheDTO.Base64Image))
            {
                _logger.LogWarning("CreateOrUpdateImageCache - Bad Request: DTO is null or missing Prompt/Base64Image.");
                stopwatch.Stop();
                return BadRequest(new { success = false, message = "Request body is invalid. Prompt and Base64Image are required." });
            }

            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(imageCacheDTO.Base64Image);
                if (imageBytes.Length == 0) throw new FormatException("Image data is empty after decoding.");
                _logger.LogInformation("CreateOrUpdateImageCache - Base64 decoding successful. Image size: {ImageSize} bytes for Prompt: {Prompt}", imageBytes.Length, imageCacheDTO.Prompt);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "CreateOrUpdateImageCache - Base64 decoding error for Prompt: {Prompt}", imageCacheDTO.Prompt);
                stopwatch.Stop();
                return BadRequest(new { success = false, message = $"Invalid Base64 image data: {ex.Message}" });
            }

            var hashValue = GenerateHash(imageCacheDTO.Prompt);
            _logger.LogInformation("CreateOrUpdateImageCache - Generated HashValue: {HashValue} for Prompt: {Prompt}", hashValue, imageCacheDTO.Prompt);

            try
            {
                var existingRecord = await _context.ImageCache
                    .Include(ic => ic.Product)
                    .Include(ic => ic.Supplier)
                    .FirstOrDefaultAsync(ic => ic.HashValue == hashValue);

                string actionType = existingRecord != null ? "updated" : "created";
                ImageCache recordToSave = existingRecord ?? new ImageCache { HashValue = hashValue, Status = true };

                recordToSave.Prompt = imageCacheDTO.Prompt;
                recordToSave.Image = imageBytes;
                recordToSave.Status = true; // Her zaman aktif et

                // EntityType ve EntityId'ye göre ProductID veya SupplierID ata
                recordToSave.ProductID = null; // Önce sıfırla
                recordToSave.SupplierID = null; // Önce sıfırla

                if (!string.IsNullOrEmpty(imageCacheDTO.EntityType) && imageCacheDTO.EntityId.HasValue)
                {
                    if (imageCacheDTO.EntityType.Equals("Product", StringComparison.OrdinalIgnoreCase))
                    {
                        recordToSave.ProductID = imageCacheDTO.EntityId.Value;
                        _logger.LogInformation("CreateOrUpdateImageCache - Associating with ProductID: {ProductID}", recordToSave.ProductID);
                    }
                    else if (imageCacheDTO.EntityType.Equals("Supplier", StringComparison.OrdinalIgnoreCase))
                    {
                        recordToSave.SupplierID = imageCacheDTO.EntityId.Value;
                        _logger.LogInformation("CreateOrUpdateImageCache - Associating with SupplierID: {SupplierID}", recordToSave.SupplierID);
                    }
                    else
                    {
                        _logger.LogWarning("CreateOrUpdateImageCache - Unknown EntityType: {EntityType}. No association will be made.", imageCacheDTO.EntityType);
                    }
                }
                else
                {
                    _logger.LogInformation("CreateOrUpdateImageCache - No EntityType/EntityId provided. Saving as general cache.");
                }

                if (existingRecord == null)
                {
                    _context.ImageCache.Add(recordToSave);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("CreateOrUpdateImageCache - DB record {ActionType} successfully (ID: {ID}).", actionType, recordToSave.ID);

                // Product.ImageUrl Güncellemesi (ProductID varsa ve bu resimle ilişkilendirildiyse)
                string? finalImageUrl = null;
                if (recordToSave.ProductID.HasValue)
                {
                    var product = await _context.Products.FindAsync(recordToSave.ProductID.Value);
                    if (product != null)
                    {
                        finalImageUrl = $"/api/imagecache/image/{recordToSave.ID}";
                        product.ImageUrl = finalImageUrl;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("CreateOrUpdateImageCache - Updated Product (ID: {ProductId}) ImageUrl to: {ImageUrl}", product.ProductID, finalImageUrl);
                    }
                    else
                    {
                        _logger.LogWarning("CreateOrUpdateImageCache - Product with ID {ProductId} not found. Cannot update Product.ImageUrl.", recordToSave.ProductID.Value);
                    }
                }

                _logger.LogInformation("CreateOrUpdateImageCache - Saving/Updating Redis cache for HashValue: {HashValue}", hashValue);
                await _redisService.SaveImageToCacheAsync(hashValue, imageCacheDTO.Base64Image);
                _logger.LogInformation("CreateOrUpdateImageCache - Redis cache updated/saved.");

                var responseDto = _mapper.Map<ImageCacheResponseDTO>(recordToSave);
                if (recordToSave.ProductID.HasValue && recordToSave.Product == null) // Explicit load for response if needed
                {
                    recordToSave.Product = await _context.Products.FindAsync(recordToSave.ProductID.Value);
                }
                if (recordToSave.SupplierID.HasValue && recordToSave.Supplier == null) // Explicit load for response if needed
                {
                    recordToSave.Supplier = await _context.Suppliers.FindAsync(recordToSave.SupplierID.Value);
                }
                // Mapper zaten Product.ProductName ve Supplier.SupplierName'i dolduracak (eğer navigation property'ler yüklüyse)
                // Bu yüzden DTO'yu tekrar mapleyebiliriz veya manuel doldurmaya devam edebiliriz.
                // Güvenli olması için tekrar mapleyelim veya kontrol edelim.
                var finalResponseDto = _mapper.Map<ImageCacheResponseDTO>(recordToSave); // Product/Supplier yüklendikten sonra tekrar map'le
                finalResponseDto.Base64Image = imageCacheDTO.Base64Image;
                finalResponseDto.ImageUrl = $"/api/imagecache/image/{recordToSave.ID}";

                stopwatch.Stop();
                _logger.LogInformation("END CreateOrUpdateImageCache - Record {ActionType}. Returning OK. Total time: {ElapsedMilliseconds}ms", actionType, stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, data = finalResponseDto, message = $"Image cache record {actionType} successfully.", source = actionType });
            }
            catch (DbUpdateException dbEx)
            {
                stopwatch.Stop();
                _logger.LogError(dbEx, "ERROR CreateOrUpdateImageCache (DbUpdateException) - Prompt: {Prompt}. Inner: {Inner}", imageCacheDTO.Prompt, dbEx.InnerException?.Message);
                return StatusCode(500, new { success = false, message = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR CreateOrUpdateImageCache (General Exception) - Prompt: {Prompt}", imageCacheDTO.Prompt);
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/imagecache/products - ProductID'si olan tüm cache kayıtlarını listeler
        [HttpGet("products")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageCacheByProducts()
        {
            _logger.LogInformation("START GetImageCacheByProducts");
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var imageCaches = await _context.ImageCache
                    .Where(ic => ic.ProductID != null && ic.Status == true)
                    .Include(ic => ic.Product) // Product bilgilerini çekmek için
                    .OrderByDescending(ic => ic.ID) // En son eklenenler üste gelsin
                    .ToListAsync();

                var response = _mapper.Map<List<ImageCacheResponseDTO>>(imageCaches);
                // ImageUrl mapper tarafından zaten set ediliyor. Base64Image'e gerek yok listelerde.

                stopwatch.Stop();
                _logger.LogInformation("END GetImageCacheByProducts - Found {Count} records. Total time: {ElapsedMilliseconds}ms", response.Count, stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageCacheByProducts. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        // GET: api/imagecache/product/{productId} - Belirli bir ProductID'ye ait cache kayıtlarını listeler
        [HttpGet("product/{productId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageCacheForProduct(int productId)
        {
            _logger.LogInformation("START GetImageCacheForProduct - ProductID: {ProductID}", productId);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var imageCaches = await _context.ImageCache
                    .Where(ic => ic.ProductID == productId && ic.Status == true)
                    .Include(ic => ic.Product)
                    .OrderByDescending(ic => ic.ID)
                    .ToListAsync();

                if (!imageCaches.Any())
                {
                    _logger.LogWarning("GetImageCacheForProduct - No active images found for ProductID: {ProductID}", productId);
                    // return NotFound(new { success = false, message = $"No active images found for ProductID {productId}." });
                    // Boş liste dönmek daha iyi olabilir.
                }
                
                var response = _mapper.Map<List<ImageCacheResponseDTO>>(imageCaches);

                stopwatch.Stop();
                _logger.LogInformation("END GetImageCacheForProduct - Found {Count} records for ProductID: {ProductID}. Total time: {ElapsedMilliseconds}ms", response.Count, productId, stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageCacheForProduct - ProductID: {ProductID}. Total time: {ElapsedMilliseconds}ms", productId, stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        // GET: api/imagecache/suppliers - SupplierID'si olan tüm cache kayıtlarını listeler
        [HttpGet("suppliers")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageCacheBySuppliers()
        {
            _logger.LogInformation("START GetImageCacheBySuppliers");
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var imageCaches = await _context.ImageCache
                    .Where(ic => ic.SupplierID != null && ic.Status == true)
                    .Include(ic => ic.Supplier) // Supplier bilgilerini çekmek için
                    .OrderByDescending(ic => ic.ID)
                    .ToListAsync();

                var response = _mapper.Map<List<ImageCacheResponseDTO>>(imageCaches);

                stopwatch.Stop();
                _logger.LogInformation("END GetImageCacheBySuppliers - Found {Count} records. Total time: {ElapsedMilliseconds}ms", response.Count, stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageCacheBySuppliers. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        // GET: api/imagecache/supplier/{supplierId} - Belirli bir SupplierID'ye ait cache kayıtlarını listeler
        [HttpGet("supplier/{supplierId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageCacheForSupplier(int supplierId)
        {
            _logger.LogInformation("START GetImageCacheForSupplier - SupplierID: {SupplierID}", supplierId);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var imageCaches = await _context.ImageCache
                    .Where(ic => ic.SupplierID == supplierId && ic.Status == true)
                    .Include(ic => ic.Supplier)
                    .OrderByDescending(ic => ic.ID)
                    .ToListAsync();
                
                var response = _mapper.Map<List<ImageCacheResponseDTO>>(imageCaches);

                stopwatch.Stop();
                _logger.LogInformation("END GetImageCacheForSupplier - Found {Count} records for SupplierID: {SupplierID}. Total time: {ElapsedMilliseconds}ms", response.Count, supplierId, stopwatch.ElapsedMilliseconds);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "ERROR GetImageCacheForSupplier - SupplierID: {SupplierID}. Total time: {ElapsedMilliseconds}ms", supplierId, stopwatch.ElapsedMilliseconds);
                return StatusCode(500, new { success = false, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        // DELETE: api/imagecache/{id} - Bir cache kaydını soft delete eder (Status=false) ve Redis'ten siler
        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> SoftDeleteImageCache(int id) // Metod adı daha açıklayıcı
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("START SoftDeleteImageCache - ID: {ID}", id);

            var imageCache = await _context.ImageCache.FindAsync(id);

            if (imageCache == null)
            {
                _logger.LogWarning("SoftDeleteImageCache - Not Found: Record with ID {ID} not found.", id);
                stopwatch.Stop();
                return NotFound(new { success = false, message = $"Image cache record with ID {id} not found." });
            }

            _logger.LogInformation("SoftDeleteImageCache - Found record (ID: {ID}). Current Status: {Status}, HashValue: {HashValue}", imageCache.ID, imageCache.Status, imageCache.HashValue);

            // Redis'ten sil (varsa)
            if (!string.IsNullOrEmpty(imageCache.HashValue))
            {
                _logger.LogInformation("SoftDeleteImageCache - Deleting from Redis cache. HashValue: {HashValue}", imageCache.HashValue);
                await _redisService.DeleteFromCacheAsync(imageCache.HashValue);
                _logger.LogInformation("SoftDeleteImageCache - Redis delete completed.");
            }

            // DB'de durumu güncelle (Soft delete)
            imageCache.Status = false;
            _logger.LogInformation("SoftDeleteImageCache - Updating DB record status to false for ID: {ID}", id);
            await _context.SaveChangesAsync();
            _logger.LogInformation("SoftDeleteImageCache - DB status updated.");
            
            stopwatch.Stop();
            _logger.LogInformation("END SoftDeleteImageCache - Completed successfully. Total time: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            return Ok(new { success = true, message = $"Image cache record with ID {id} marked as deleted." });
        }
    }
}
