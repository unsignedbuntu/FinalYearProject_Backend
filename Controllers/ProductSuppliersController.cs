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
    public class ProductSuppliersController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public ProductSuppliersController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetProductSuppliers()
        {
            var productSuppliers = _context.ProductSuppliers
                .Include(ps => ps.Product)
                .Include(ps => ps.Supplier)
                .Where(ps => ps.Status == true)
                .Select(ps => _mapper.Map<ProductSuppliers>(ps))
                .ToList();

            return Ok(productSuppliers);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetProductSuppliersByID(int id)
        {
            var productSuppliers = _context.ProductSuppliers
                .Include(ps => ps.Product)
                .Include(ps => ps.Supplier)
                .Where(ps => ps.ProductSupplierID == id)
                .Where(ps => ps.Status == true)
                .Select(ps => _mapper.Map<ProductSuppliers>(ps))
                .FirstOrDefault();

            if (productSuppliers == null)
            {
                return NotFound();
            }

            return Ok(productSuppliers);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateProductSuppliers([FromBody] ProductSuppliersResponseDTO productSuppliersResponseDTO)
        {
            if (productSuppliersResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productSuppliersResponse = _mapper.Map<ProductSuppliers>(productSuppliersResponseDTO);

            _context.ProductSuppliers.Add(productSuppliersResponse);
            _context.SaveChanges();

            return Ok(productSuppliersResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateProductSuppliers(int id, [FromBody] ProductSuppliersResponseDTO productSuppliersResponseDTO)
        {
            if (productSuppliersResponseDTO == null)
            {
                return BadRequest();
            }

            var productSuppliersResponse = _context.ProductSuppliers.FirstOrDefault(ps => ps.ProductSupplierID == id);

            if (productSuppliersResponse == null)
            {
                return NotFound();
            }

            productSuppliersResponse.SupplyPrice = productSuppliersResponseDTO.SupplyPrice;

            productSuppliersResponse.SupplyDate = productSuppliersResponseDTO.SupplyDate;

            productSuppliersResponse.ProductID = productSuppliersResponseDTO.ProductID;

            productSuppliersResponse.SupplierID = productSuppliersResponseDTO.SupplierID;

            _context.SaveChanges();

            return Ok(productSuppliersResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteProductSuppliersByStatus(int id)
        {
            var productSuppliers = _context.ProductSuppliers.FirstOrDefault(ps => ps.ProductSupplierID == id);

            if (productSuppliers == null)
            {
                return NotFound();
            }


            productSuppliers.Status = false;


            _context.ProductSuppliers.Update(productSuppliers);
            _context.SaveChanges();

            return NoContent();
        }

    }
}


