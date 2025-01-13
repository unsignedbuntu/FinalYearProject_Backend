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
    public class StoresController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public StoresController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetStores()
        {
            var stores = _context.Stores
                .Where(st => st.Status == true)
                .Select(st => _mapper.Map<Stores>(st))
                .ToList();

            return Ok(stores);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetStoresByID(int id)
        {
            var stores = _context.Stores
                .Where(st => st.StoreID == id)
                .Where(st => st.Status == true)
                .Select(st => _mapper.Map<Stores>(st))
                .FirstOrDefault();

            if (stores == null)
            {
                return NotFound();
            }

            return Ok(stores);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateStores([FromBody] StoresResponseDTO storesResponseDTO)
        {
            if (storesResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var storesResponse = _mapper.Map<Stores>(storesResponseDTO);

            _context.Stores.Add(storesResponse);
            _context.SaveChanges();

            return Ok(storesResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateStores(int id, [FromBody] StoresResponseDTO storesResponseDTO)
        {
            if (storesResponseDTO == null)
            {
                return BadRequest();
            }

            var storesResponse = _context.Stores.FirstOrDefault(st => st.StoreID == id);

            if (storesResponse == null)
            {
                return NotFound();
            }

            storesResponse.StoreName = storesResponseDTO.StoreName;


            _context.SaveChanges();

            return Ok(storesResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteStoresByStatus(int id)
        {
            var stores = _context.Stores.FirstOrDefault(st => st.StoreID == id);

            if (stores == null)
            {
                return NotFound();
            }


            stores.Status = false;


            _context.Stores.Update(stores);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

