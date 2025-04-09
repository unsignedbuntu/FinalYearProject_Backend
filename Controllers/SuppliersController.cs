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
    public class SuppliersController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public SuppliersController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetSuppliers()
        {
            var suppliers = _context.Suppliers
                .Where(s => s.Status == true)
                .Select(s => _mapper.Map<SuppliersResponseDTO>(s))
                .ToList();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetSupplierByID(int id)
        {
            var supplier = _context.Suppliers
                .Where(s => s.SupplierID == id && s.Status == true)
                .Select(s => _mapper.Map<SuppliersResponseDTO>(s))
                .FirstOrDefault();

            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateSupplier([FromBody] SuppliersDTO supplierDTO)
        {
            if (supplierDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = _mapper.Map<Suppliers>(supplierDTO);
            supplier.Status = true;

            _context.Suppliers.Add(supplier);
            _context.SaveChanges();

            var supplierResponse = _mapper.Map<SuppliersResponseDTO>(supplier);
            return Ok(supplierResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateSupplier(int id, [FromBody] SuppliersDTO supplierDTO)
        {
            if (supplierDTO == null)
            {
                return BadRequest();
            }

            var supplier = _context.Suppliers.FirstOrDefault(s => s.SupplierID == id);

            if (supplier == null)
            {
                return NotFound();
            }

            supplier.SupplierName = supplierDTO.SupplierName;
            supplier.ContactEmail = supplierDTO.ContactEmail;

            _context.SaveChanges();

            var supplierResponse = _mapper.Map<SuppliersResponseDTO>(supplier);
            return Ok(supplierResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteSuppliersByStatus(int id)
        {
            var suppliers = _context.Suppliers.FirstOrDefault(su => su.SupplierID == id);

            if (suppliers == null)
            {
                return NotFound();
            }


            suppliers.Status = false;


            _context.Suppliers.Update(suppliers);
            _context.SaveChanges();

            return NoContent();
        }

    }
}




