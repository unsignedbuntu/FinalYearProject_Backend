using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using KTUN_Final_Year_Project.DTOs.Products;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public ProductsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetProducts()
        {
            var products = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Include(p => p.InventoryRecords)
                .Where(p => p.Status == true)
                .Select(p => _mapper.Map<ProductsResponseDTO>(p))
                .ToList();
                
            return Ok(products);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetProductByID(int id)
        {
            var product = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Include(p => p.InventoryRecords)
                .FirstOrDefault(p => p.ProductID == id && p.Status == true);

            if (product == null)
            {
                return NotFound();
            }

            var productResponse = _mapper.Map<ProductsResponseDTO>(product);
            return Ok(productResponse);
        }

        [HttpGet("ByCategory/{categoryId}")]
        [Produces("application/json")]
        public IActionResult GetProductsByCategoryID(int categoryId)
        {
            var products = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Include(p => p.InventoryRecords)
                .Where(p => p.CategoryID == categoryId)
                .Where(p => p.Status == true)
                .Select(p => _mapper.Map<ProductsResponseDTO>(p))
                .ToList();

            return Ok(products);
        }

        [HttpGet("ByStore/{storeId}")]
        [Produces("application/json")]
        public IActionResult GetProductsByStoreID(int storeId)
        {
            var products = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Include(p => p.InventoryRecords)
                .Where(p => p.StoreID == storeId)
                .Where(p => p.Status == true)
                .Select(p => _mapper.Map<ProductsResponseDTO>(p))
                .ToList();

            return Ok(products);
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateProduct([FromBody] ProductsDTO productDTO)
        {
            if (productDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Barkod benzersizliğini kontrol et (aktif ürünler arasında)
            if (!string.IsNullOrEmpty(productDTO.Barcode))
            {
                var existingProduct = _context.Products
                    .Where(p => p.Barcode == productDTO.Barcode && p.Status == true)
                    .FirstOrDefault();
                
                if (existingProduct != null)
                {
                    return BadRequest("Bu barkod zaten kullanımda.");
                }
            }

            var newProduct = _mapper.Map<Products>(productDTO);
            
            // Kaldırılan alanların atanması kaldırıldı
            // newProduct.CreatedDate = DateTime.Now;
            newProduct.Status = true;

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            var productResponse = _mapper.Map<ProductsResponseDTO>(newProduct);
            return Ok(productResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductsDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest();
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && p.Status == true);

            if (product == null)
            {
                return NotFound();
            }

            // Barkod benzersizliğini kontrol et (diğer aktif ürünler arasında)
            if (!string.IsNullOrEmpty(productDTO.Barcode))
            {
                var existingProduct = _context.Products
                    .Where(p => p.Barcode == productDTO.Barcode && p.ProductID != id && p.Status == true)
                    .FirstOrDefault();
                
                if (existingProduct != null)
                {
                    return BadRequest("Bu barkod zaten başka bir ürün için kullanımda.");
                }
            }

            // Ürünü güncelle
            product.ProductName = productDTO.ProductName;
            // Kaldırılan alanların güncellenmesi kaldırıldı
            // product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            // product.ImageUrl = productDTO.ImageUrl;
            product.CategoryID = productDTO.CategoryID;
            product.StoreID = productDTO.StoreID;
            product.StockQuantity = productDTO.StockQuantity;
            product.Barcode = productDTO.Barcode;

            _context.SaveChanges();

            var productResponse = _mapper.Map<ProductsResponseDTO>(product);
            return Ok(productResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteProductByStatus(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && p.Status == true);

            if (product == null)
            {
                return NotFound();
            }

            product.Status = false;

            _context.Products.Update(product);
            _context.SaveChanges();

            return NoContent();
        }

        // Yeni endpoint: En çok incelenen ürünleri getirir
        [HttpGet("top-reviewed")]
        public async Task<ActionResult<IEnumerable<TopReviewedProductDto>>> GetTopReviewedProducts([FromQuery] int count = 4)
        {
            if (count <= 0)
            {
                count = 4; // Geçersiz count sağlanırsa varsayılan olarak 4
            }
            if (count > 20) // İsteğe bağlı: Kötüye kullanımı önlemek için maksimum count sınırı
            {
                count = 20;
            }

            try
            {
                var topProducts = await _context.Products
                    .Where(p => p.Status == true && p.Reviews.Any(r => r.Status == true)) // Sadece aktif ürünleri ve aktif yorumları dikkate al
                    .Select(p => new
                    {
                        Product = p,
                        ActiveReviews = p.Reviews.Where(r => r.Status == true).ToList() // Hesaplama için aktif yorumları filtrele
                    })
                    .Select(pData => new TopReviewedProductDto
                    {
                        ProductID = pData.Product.ProductID,
                        ProductName = pData.Product.ProductName,
                        Price = pData.Product.Price,
                        OriginalPrice = null, // Product entity'sinde OriginalPrice yok, bu yüzden null atanıyor.
                                              // Eğer eklenirse: pData.Product.OriginalPrice,
                        ImageUrl = pData.Product.ImageUrl ?? "/images/placeholder.png", // Null ise bir placeholder sağlayın
                        AverageRating = pData.ActiveReviews.Any() ? pData.ActiveReviews.Average(r => r.Rating) : 0,
                        ReviewCount = pData.ActiveReviews.Count()
                    })
                    .OrderByDescending(p => p.ReviewCount) // Yorum sayısına göre sırala
                    .ThenByDescending(p => p.AverageRating) // İsteğe bağlı olarak, sonra ortalama puana göre
                    .Take(count)
                    .ToListAsync();

                if (topProducts == null || !topProducts.Any())
                {
                    return NotFound(new { Message = "En çok incelenen ürün bulunamadı." });
                }

                return Ok(topProducts);
            }
            catch (System.Exception ex)
            {
                // Gerçek bir uygulamada burada bir logger kullanmalısınız (örneğin, ILogger enjekte ederek)
                // Console.WriteLine($"Error in GetTopReviewedProducts: {ex.ToString()}"); // veya logger.LogError(ex, "Error in GetTopReviewedProducts");
                return StatusCode(500, new { Message = "En çok incelenen ürünler alınırken bir hata oluştu.", Details = ex.Message });
            }
        }
    }
}


