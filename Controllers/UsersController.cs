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
    public class UsersController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public UsersController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Where(u => u.Status == true)
                .Select(u => _mapper.Map<Users>(u))
                .ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetUsersByID(int id)
        {
            var users = _context.Users
                 .Where(u => u.Id == id)
                .Where(u => u.Status == true)
                .Select(u => _mapper.Map<Users>(u))
                .FirstOrDefault();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateUsers([FromBody] UsersResponseDTO usersResponseDTO)
        {
            if (usersResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usersResponse = _mapper.Map<Users>(usersResponseDTO);

            _context.Users.Add(usersResponse);
            _context.SaveChanges();

            return Ok(usersResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateUsers(int id, [FromBody] UsersResponseDTO usersResponseDTO)
        {
            if (usersResponseDTO == null)
            {
                return BadRequest();
            }

            var usersResponse = _context.Users.FirstOrDefault(u => u.Id == id);

            if (usersResponse == null)
            {
                return NotFound();
            }

            usersResponse.FullName = usersResponseDTO.FullName;

            usersResponse.Email = usersResponseDTO.Email;

            usersResponse.PhoneNumber = usersResponseDTO.PhoneNumber;

            usersResponse.Address = usersResponseDTO.Address;

            usersResponse.NFC_CardID = usersResponseDTO.NFC_CardID;


            _context.SaveChanges();

            return Ok(usersResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteUsersByStatus(int id)
        {
            var users = _context.Users.FirstOrDefault(u => u.Id == id);

            if (users == null)
            {
                return NotFound();
            }


            users.Status = false;


            _context.Users.Update(users);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

