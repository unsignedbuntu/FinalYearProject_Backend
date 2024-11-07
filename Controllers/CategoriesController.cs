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
                .Where(c => c.Status == true)
                .Select(c => _mapper.Map<Categories>(c))
                .ToList();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetCategoriesByID(int id)
        {
            var categories = _context.Categories
                 .Where(c => c.CategoryID == id)
                .Where(c => c.Status == true)
                .Select(c => _mapper.Map<Categories>(c))
                .FirstOrDefault();

            if (categories == null)
            {
                return NotFound();
            }

            return Ok(categories);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateCategories([FromBody] CategoriesResponseDTO categoriesResponseDTO)
        {
            if (categoriesResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriesResponse = _mapper.Map<Categories>(categoriesResponseDTO);

            _context.Categories.Add(categoriesResponse);
            _context.SaveChanges();

            return Ok(categoriesResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateCategories(int id, [FromBody] CategoriesResponseDTO categoriesResponseDTO)
        {
            if (categoriesResponseDTO == null)
            {
                return BadRequest();
            }

            var categoriesResponse = _context.Categories.FirstOrDefault(c => c.CategoryID == id);

            if (categoriesResponse == null)
            {
                return NotFound();
            }

            categoriesResponse.CategoryName = categoriesResponseDTO.CategoryName;
       


            _context.SaveChanges();

            return Ok(categoriesResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteCategoriesByStatus(int id)
        {
            var categories = _context.Categories.FirstOrDefault(c => c.CategoryID == id);

            if (categories == null)
            {
                return NotFound();
            }


            categories.Status = false;


            _context.Categories.Update(categories);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

