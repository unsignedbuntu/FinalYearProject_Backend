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
                .Where(s => s.Status == true)
                .Select(s => _mapper.Map<StoresResponseDTO>(s))
                .ToList();
            return Ok(stores);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetStoreByID(int id)
        {
            var store = _context.Stores
                .Where(s => s.StoreID == id && s.Status == true)
                .Select(s => _mapper.Map<StoresResponseDTO>(s))
                .FirstOrDefault();

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateStore([FromBody] StoresDTO storeDTO)
        {
            if (storeDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store = _mapper.Map<Stores>(storeDTO);
            store.Status = true;

            _context.Stores.Add(store);
            _context.SaveChanges();

            var storeResponse = _mapper.Map<StoresResponseDTO>(store);
            return Ok(storeResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateStore(int id, [FromBody] StoresDTO storeDTO)
        {
            if (storeDTO == null)
            {
                return BadRequest();
            }

            var store = _context.Stores.FirstOrDefault(s => s.StoreID == id);

            if (store == null)
            {
                return NotFound();
            }

            store.StoreName = storeDTO.StoreName;

            _context.SaveChanges();

            var storeResponse = _mapper.Map<StoresResponseDTO>(store);
            return Ok(storeResponse);
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

