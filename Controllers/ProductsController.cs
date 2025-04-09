using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    }
}


