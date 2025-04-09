using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace KTUN_Final_Year_Project.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Users> _userManager;
        public UsersController(KTUN_DbContext context, IMapper mapper, UserManager<Users> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetUsers()
        {
            // DİKKAT: Users entity'sinin TÜM alanları (hassas olanlar dahil) döndürülüyor.
            var users = await _userManager.Users
                .Where(u => u.Status == true) // Aktif kullanıcıları filtrele
                // .Select(u => _mapper.Map<UsersResponseDTO>(u)) // Mapping kaldırıldı
                .ToListAsync();

            // Roller gibi ek bilgiler burada çekilip entity'e eklenemez (entity değiştirilemez).
            // İstemci tarafında /api/users/{id}/roles gibi ayrı bir endpoint çağrılmalı.

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            // DİKKAT: Users entity'sinin TÜM alanları (hassas olanlar dahil) döndürülüyor.
            var user = await _userManager.FindByIdAsync(id.ToString()); // Doğrudan Users nesnesini al

            // Aktif olmayan veya bulunamayan kullanıcı kontrolü
            if (user == null || !user.Status) 
            {
                return NotFound();
            }
            
            // ResponseDTO mapping kaldırıldı, user nesnesi doğrudan döndürülüyor.

            return Ok(user);
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

