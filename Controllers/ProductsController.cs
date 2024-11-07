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
    [Route("[controller]")]
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
            var product = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Where(p => p.Status == true)
                .Select(p => _mapper.Map<Products>(p))
                .ToList();

            return Ok(product);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetProductsByID(int id)
        {
            var product = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Where(p => p.ProductID == id)
                .Where(p => p.Status == true)
                .Select(p => _mapper.Map<Products>(p))
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateProducts([FromBody] ProductsResponseDTO productsResponseDTO)
        {
            if (productsResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productResponse = _mapper.Map<Products>(productsResponseDTO);

            _context.Products.Add(productResponse);
            _context.SaveChanges();

            return Ok(productResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateProducts(int id, [FromBody] ProductsResponseDTO productsResponseDTO)
        {
            if (productsResponseDTO == null)
            {
                return BadRequest();
            }

            var productResponse = _context.Products.FirstOrDefault(p => p.ProductID == id);

            if (productResponse == null)
            {
                return NotFound();
            }
            
            productResponse.ProductName = productsResponseDTO.ProductName;

            productResponse.Price = productsResponseDTO.Price;

            productResponse.StockQuantity = productsResponseDTO.StockQuantity;

            productResponse.Barcode = productsResponseDTO.Barcode;

            productResponse.StoreID = productsResponseDTO.StoreID;

            productResponse.CategoryID = productsResponseDTO.CategoryID;

            _context.SaveChanges();

            return Ok(productResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteProductsByStatus(int id)
        {
            var products = _context.Products.FirstOrDefault(p => p.ProductID == id);

            if (products == null)
            {
                return NotFound();
            }


            products.Status = false;


            _context.Products.Update(products);
            _context.SaveChanges();

            return NoContent();
        }

    }
}


