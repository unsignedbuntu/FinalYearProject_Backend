using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories
                .Include(c => c.Store)
                .Where(c => c.Status == true)
                .Select(c => _mapper.Map<CategoriesResponseDTO>(c))
                .ToList();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetCategoryByID(int id)
        {
            var category = _context.Categories
                .Include(c => c.Store)
                .Where(c => c.CategoryID == id)
                .Where(c => c.Status == true)
                .Select(c => _mapper.Map<CategoriesResponseDTO>(c))
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpGet("ByStore/{storeId}")]
        [Produces("application/json")]
        public IActionResult GetCategoriesByStoreID(int storeId)
        {
            var categories = _context.Categories
                .Include(c => c.Store)
                .Where(c => c.StoreID == storeId)
                .Where(c => c.Status == true)
                .Select(c => _mapper.Map<CategoriesResponseDTO>(c))
                .ToList();

            if (categories.Count == 0)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateCategory([FromBody] CategoriesDTO categoryDTO)
        {
            if (categoryDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Categories>(categoryDTO);
            
            category.Status = true;

            _context.Categories.Add(category);
            _context.SaveChanges();

            var categoryResponse = _mapper.Map<CategoriesResponseDTO>(category);
            return Ok(categoryResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoriesDTO categoryDTO)
        {
            if (categoryDTO == null)
            {
                return BadRequest();
            }

            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName = categoryDTO.CategoryName;
            category.StoreID = categoryDTO.StoreID;

            _context.SaveChanges();

            var categoryResponse = _mapper.Map<CategoriesResponseDTO>(category);
            return Ok(categoryResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteCategoryByStatus(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            category.Status = false;

            _context.Categories.Update(category);
            _context.SaveChanges();

            return NoContent();
        }
    }
} 