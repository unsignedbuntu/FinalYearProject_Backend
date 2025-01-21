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
    public class UserStoreController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public UserStoreController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetUserStore()
        {
            var userStore = _context.UserStore
                .Include(us => us.User)
                .Include(us => us.Store)
                .Where(us => us.Status == true)
                .Select(us => _mapper.Map<UserStore>(us))
                .ToList();

            return Ok(userStore);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetUserStoreByID(int id)
        {
            var userStore = _context.UserStore
                .Include(us => us.User)
                .Include(us => us.Store)
                .Where(us => us.UserStoreID == id)
                .Where(us => us.Status == true)
                .Select(us => _mapper.Map<UserStore>(us))
                .FirstOrDefault();

            if (userStore == null)
            {
                return NotFound();
            }

            return Ok(userStore);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateUserStore([FromBody] UserStoreResponseDTO userStoreResponseDTO)
        {
            if (userStoreResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userStoreResponse = _mapper.Map<UserStore>(userStoreResponseDTO);

            _context.UserStore.Add(userStoreResponse);
            _context.SaveChanges();

            return Ok(userStoreResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateUserStore(int id, [FromBody] UserStoreResponseDTO userStoreResponseDTO)
        {
            if (userStoreResponseDTO == null)
            {
                return BadRequest();
            }

            var userStoreResponse = _context.UserStore.FirstOrDefault(us => us.UserStoreID == id);

            if (userStoreResponse == null)
            {
                return NotFound();
            }

            userStoreResponse.EnrollmentDate = userStoreResponseDTO.EnrollmentDate;

            userStoreResponse.UserID = userStoreResponseDTO.UserID;

            userStoreResponse.StoreID = userStoreResponseDTO.StoreID;


            _context.SaveChanges();

            return Ok(userStoreResponse);
        }


        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteUserStoreByStatus(int id)
        {
            var userStore = _context.UserStore.FirstOrDefault(us => us.UserStoreID == id);

            if (userStore == null)
            {
                return NotFound();
            }


            userStore.Status = false;


            _context.UserStore.Update(userStore);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

