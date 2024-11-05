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
    public class UserLoyaltyController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public UserLoyaltyController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetUserLoyalty()
        {
            var userLoyalty = _context.UserLoyalty
                .Include(ul => ul.LoyaltyProgram)
                .Include(ul => ul.User)
                .Where(ul => ul.Status == true)
                .Select(ul => _mapper.Map<UserLoyalty>(ul))
                .ToList();

            return Ok(userLoyalty);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetUserLoyaltyByID(int id)
        {
            var userLoyalty = _context.UserLoyalty
                .Include(ul => ul.LoyaltyProgram)
                .Include(ul => ul.User)
                .Where(ul => ul.UserLoyaltyID == id)
                .Where(ul => ul.Status == true)
                .Select(ul => _mapper.Map<UserLoyaltyDTO>(ul))
                .FirstOrDefault();

            if (userLoyalty == null)
            {
                return NotFound();
            }

            return Ok(userLoyalty);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateUserLoyalty([FromBody] UserLoyaltyResponseDTO userLoyaltyResponseDTO)
        {
            if (userLoyaltyResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userLoyaltyResponse = _mapper.Map<UserLoyalty>(userLoyaltyResponseDTO);

            _context.UserLoyalty.Add(userLoyaltyResponse);
            _context.SaveChanges();

            return Ok(userLoyaltyResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateUserLoyalty(int id, [FromBody] UserLoyaltyResponseDTO userLoyaltyResponseDTO)
        {
            if (userLoyaltyResponseDTO == null)
            {
                return BadRequest();
            }

            var userLoyaltyResponse = _context.UserLoyalty.FirstOrDefault(ul => ul.UserLoyaltyID == id);

            if (userLoyaltyResponse == null)
            {
                return NotFound();
            }

            userLoyaltyResponse.AccumulatedPoints = userLoyaltyResponseDTO.AccumulatedPoints;

            userLoyaltyResponse.EnrollmentDate = userLoyaltyResponseDTO.EnrollmentDate;


            _context.SaveChanges();

            return Ok(userLoyaltyResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteUserLoyaltyByStatus(int id)
        {
            var userLoyalty = _context.UserLoyalty.FirstOrDefault(ul => ul.UserLoyaltyID == id);

            if (userLoyalty == null)
            {
                return NotFound();
            }


            userLoyalty.Status = false;


            _context.UserLoyalty.Update(userLoyalty);
            _context.SaveChanges();

            return NoContent();
        }

    }
}


