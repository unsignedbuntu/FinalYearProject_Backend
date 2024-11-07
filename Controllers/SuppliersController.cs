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
                .Where(su => su.Status == true)
                .Select(su => _mapper.Map<Suppliers>(su))
                .ToList();

            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetSuppliersByID(int id)
        {
            var suppliers = _context.Suppliers
                .Where(su => su.SupplierID == id)
                .Where(su => su.Status == true)
                .Select(su => _mapper.Map<Suppliers>(su))
                .FirstOrDefault();

            if (suppliers == null)
            {
                return NotFound();
            }

            return Ok(suppliers);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateSuppliers([FromBody] SuppliersResponseDTO suppliersResponseDTO)
        {
            if (suppliersResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suppliersResponse = _mapper.Map<Suppliers>(suppliersResponseDTO);

            _context.Suppliers.Add(suppliersResponse);
            _context.SaveChanges();

            return Ok(suppliersResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateSuppliers(int id, [FromBody] SuppliersResponseDTO suppliersResponseDTO)
        {
            if (suppliersResponseDTO == null)
            {
                return BadRequest();
            }

            var suppliersResponse = _context.Suppliers.FirstOrDefault(su => su.SupplierID == id);

            if (suppliersResponse == null)
            {
                return NotFound();
            }

             suppliersResponse.SupplierName = suppliersResponseDTO.SupplierName;

            suppliersResponse.ContactEmail = suppliersResponseDTO.ContactEmail;


            _context.SaveChanges();

            return Ok(suppliersResponse);
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




