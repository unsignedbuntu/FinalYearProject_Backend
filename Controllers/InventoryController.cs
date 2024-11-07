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
    public class InventoryController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public InventoryController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetInventory()
        {
            var inventory = _context.Inventory
                .Include(i => i.Product)
                .Where(i => i.Status == true)
                .Select(i=> _mapper.Map<Inventory>(i))
                .ToList();

            return Ok(inventory);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetInventoryByID(int id)
        {
            var inventory = _context.Inventory
                .Include(i => i.Product)
                .Where(i => i.InventoryID == id)
                .Where(i => i.Status == true)
                .Select(i => _mapper.Map<Inventory>(i))
                .FirstOrDefault();

            if (inventory== null)
            {
                return NotFound();
            }

            return Ok(inventory);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateInventory([FromBody] InventoryResponseDTO inventoryResponseDTO)
        {
            if (inventoryResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryResponse = _mapper.Map<Inventory>(inventoryResponseDTO);

            _context.Inventory.Add(inventoryResponse);
            _context.SaveChanges();

            return Ok(inventoryResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateInventory(int id, [FromBody] InventoryResponseDTO inventoryResponseDTO)
        {
            if (inventoryResponseDTO == null)
            {
                return BadRequest();
            }

            var inventoryResponse = _context.Inventory.FirstOrDefault(i => i.InventoryID  == id);

            if (inventoryResponse == null)
            {
                return NotFound();
            }

         //   inventoryResponse.ChangeType = (Inventory.ChangeTypeEnum)inventoryResponseDTO.ChangeType;

            inventoryResponse.ChangeType = inventoryResponseDTO.ChangeType;

            inventoryResponse.QuantityChanged = inventoryResponseDTO.QuantityChanged;

            inventoryResponse.ChangeDate = inventoryResponseDTO.ChangeDate;

            inventoryResponse.ProductID = inventoryResponseDTO.ProductID;

            _context.SaveChanges();

            return Ok(inventoryResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteInventoryByStatus(int id)
        {
            var inventory = _context.Inventory.FirstOrDefault(i => i.InventoryID == id);

            if (inventory == null)
            {
                return NotFound();
            }


            inventory.Status = false;


            _context.Inventory.Update(inventory);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

