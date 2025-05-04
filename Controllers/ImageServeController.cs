using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
// using DAL; // Bu satır kaldırıldı, KTUN_DbContext farklı bir namespace'de olabilir.
using System.Text; // For Encoding
using Microsoft.EntityFrameworkCore; // Eklendi
using Microsoft.Extensions.Logging; // Eklendi

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageServeController : ControllerBase
    {
        // private readonly IDistributedCache _cache; // Redis'e productId ile erişim mantıklı değil
        private readonly KTUN_DbContext _context;
        private readonly ILogger<ImageServeController> _logger;

        public ImageServeController(/*IDistributedCache cache,*/ KTUN_DbContext context, ILogger<ImageServeController> logger)
        {
            // _cache = cache;
            _context = context;
            _logger = logger;
        }

        // GET api/ImageServe/{productId}
        [HttpGet("{productId}")]
        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetImage(int productId)
        {
            byte[]? imageData = null;

            try
            {
                // Doğrudan ImageCache tablosundan ProductID ile sorgula
                var imageCacheEntry = await _context.ImageCache
                    .FirstOrDefaultAsync(ic => ic.ProductID == productId && ic.Image != null);

                if (imageCacheEntry != null)
                {
                    imageData = imageCacheEntry.Image;
                    _logger.LogInformation("GetImage - Found image in DB cache for ProductID: {ProductId}", productId);
                }
                else
                {
                    // Belki Product tablosundaki ImageUrl'e bakılabilir (eğer dışarıdan bir URL ise)?
                    // Veya direkt NotFound dönülebilir.
                    _logger.LogWarning("GetImage - Image not found in DB cache for ProductID: {ProductId}", productId);
                    return NotFound("Image not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetImage - Error fetching image for ProductID: {ProductId}", productId);
                return StatusCode(500, "Error retrieving image.");
            }

            if (imageData == null)
            {
                // This case should ideally not be reached due to checks inside try block,
                // but adding for robustness and to satisfy nullability analysis.
                 _logger.LogWarning("GetImage - imageData is unexpectedly null before returning FileResult for ProductID: {ProductId}", productId);
                return NotFound("Image data could not be retrieved.");
            }

            // Determine content type (assuming JPEG for now, adjust if needed)
            string contentType = "image/jpeg"; // TODO: Content type bilgisini de saklamak gerekebilir

            return File(imageData, contentType);
        }
    }
} 