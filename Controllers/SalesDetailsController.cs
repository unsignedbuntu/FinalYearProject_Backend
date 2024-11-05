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
    public class SalesDetailsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public SalesDetailsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetSalesDetails()
        {
            var salesDetails = _context.SalesDetails
                .Include(sd => sd.Store)
                .Include(sd => sd.Sale)
                .Where(sd => sd.Status == true)
                .Select(sd => _mapper.Map<SalesDetails>(sd))
                .ToList();

            return Ok(salesDetails);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetSalesDetailsByID(int id)
        {
            var salesDetails = _context.SalesDetails
                .Include(sd => sd.Store)
                .Include(sd => sd.Sale)
                .Where(sd => sd.SaleDetailID == id)
                .Where(sd => sd.Status == true)
                .Select(sd => _mapper.Map<SalesDetailsDTO>(sd))
                .FirstOrDefault();

            if (salesDetails == null)
            {
                return NotFound();
            }

            return Ok(salesDetails);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateSalesDetails ([FromBody] SalesDetailsResponseDTO salesDetailsResponseDTO)
        {
            if (salesDetailsResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesDetailsResponse = _mapper.Map<SalesDetails>(salesDetailsResponseDTO);

            _context.SalesDetails.Add(salesDetailsResponse);
            _context.SaveChanges();

            return Ok(salesDetailsResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateSalesDetails(int id, [FromBody] SalesDetailsResponseDTO salesDetailsResponseDTO)
        {
            if (salesDetailsResponseDTO == null)
            {
                return BadRequest();
            }

            var salesDetailsResponse = _context.SalesDetails.FirstOrDefault(sd => sd.SaleDetailID == id);

            if (salesDetailsResponse == null)
            {
                return NotFound();
            }

            salesDetailsResponse.Quantity = salesDetailsResponseDTO.Quantity;

            salesDetailsResponse.PriceAtSale = salesDetailsResponseDTO.PriceAtSale;


            _context.SaveChanges();

            return Ok(salesDetailsResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteSalesDetailsByStatus(int id)
        {
            var salesDetails = _context.SalesDetails.FirstOrDefault(sd => sd.SaleDetailID == id);

            if (salesDetails == null)
            {
                return NotFound();
            }


            salesDetails.Status = false;


            _context.SalesDetails.Update(salesDetails);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

