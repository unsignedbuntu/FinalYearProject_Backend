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
    public class SalesController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public SalesController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetSales()
        {
            var sales = _context.Sales
                .Include(s => s.User)
                .Include(s => s.Store)
                .Where(s => s.Status == true)
                .Select(s => _mapper.Map<SalesDTO>(s))
                .ToList();

            return Ok(sales);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetSalesByID(int id)
        {
            var sales = _context.Sales
                .Include(s => s.User)
                .Include(s => s.Store)
                .Where(s => s.SaleID == id)
                .Where(s => s.Status == true)
                .Select(s => _mapper.Map<SalesDTO>(s))
                .FirstOrDefault();

            if (sales == null)
            {
                return NotFound();
            }

            return Ok(sales);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateSales([FromBody] SalesResponseDTO salesResponseDTO)
        {
            if (salesResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesResponse = _mapper.Map<Sales>(salesResponseDTO);

            _context.Sales.Add(salesResponse);
            _context.SaveChanges();

            return Ok(salesResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateSales(int id, [FromBody] SalesResponseDTO salesResponseDTO)
        {
            if (salesResponseDTO == null)
            {
                return BadRequest();
            }

            var salesResponse = _context.Sales.FirstOrDefault(s => s.SaleID == id);

            if (salesResponse == null)
            {
                return NotFound();
            }

            salesResponse.SaleDate = salesResponseDTO.SaleDate;

            salesResponse.TotalAmount = salesResponseDTO.TotalAmount;


            _context.SaveChanges();

            return Ok(salesResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteSalesByStatus(int id)
        {
            var sales = _context.Sales.FirstOrDefault(s => s.SaleID == id);

            if (sales == null)
            {
                return NotFound();
            }


            sales.Status = false;


            _context.Sales.Update(sales);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

