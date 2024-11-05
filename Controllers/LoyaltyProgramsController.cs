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
    public class LoyaltyProgramsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public LoyaltyProgramsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetLoyaltyPrograms()
        {
            var loyaltyPrograms = _context.LoyaltyPrograms
                .Where(lp => lp.Status == true)
                .Select(lp => _mapper.Map<LoyaltyProgramsDTO>(lp))
                .ToList();

            return Ok(loyaltyPrograms);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetLoyaltyProgramsByID(int id)
        {
            var loyaltyPrograms = _context.LoyaltyPrograms
                 .Where(lp => lp.LoyaltyProgramsID == id)
                .Where(lp => lp.Status == true)
                .Select(lp => _mapper.Map<LoyaltyProgramsDTO>(lp))
                .FirstOrDefault();

            if (loyaltyPrograms == null)
            {
                return NotFound();
            }

            return Ok(loyaltyPrograms);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateLoyaltyPrograms([FromBody] LoyaltyProgramsResponseDTO loyaltyProgramsResponseDTO)
        {
            if (loyaltyProgramsResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loyaltyProgramsResponse = _mapper.Map<LoyaltyPrograms>(loyaltyProgramsResponseDTO);

            _context.LoyaltyPrograms.Add(loyaltyProgramsResponse);
            _context.SaveChanges();

            return Ok(loyaltyProgramsResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateLoyaltyPrograms(int id, [FromBody] LoyaltyProgramsResponseDTO loyaltyProgramsResponseDTO)
        {
            if (loyaltyProgramsResponseDTO == null)
            {
                return BadRequest();
            }

            var loyaltyProgramsResponse = _context.LoyaltyPrograms.FirstOrDefault(lp => lp.LoyaltyProgramsID == id);

            if (loyaltyProgramsResponse == null)
            {
                return NotFound();
            }

            loyaltyProgramsResponse.ProgramName = loyaltyProgramsResponseDTO.ProgramName;

            loyaltyProgramsResponse.DiscountRate = loyaltyProgramsResponseDTO.DiscountRate;

            loyaltyProgramsResponse.PointsMultiplier = loyaltyProgramsResponseDTO.PointsMultiplier;


            _context.SaveChanges();

            return Ok(loyaltyProgramsResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteLoyaltyProgramsByStatus(int id)
        {
            var loyaltyPrograms = _context.LoyaltyPrograms.FirstOrDefault(lp => lp.LoyaltyProgramsID == id);

            if (loyaltyPrograms == null)
            {
                return NotFound();
            }


            loyaltyPrograms.Status = false;


            _context.LoyaltyPrograms.Update(loyaltyPrograms);
            _context.SaveChanges();

            return NoContent();
        }

    }
}


