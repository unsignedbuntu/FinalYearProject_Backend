using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using KTUN_Final_Year_Project.ResponseDTOs;
using System.Collections.Generic;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private const int MaxResultsPerCategory = 15;

        public SearchController(KTUN_DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearch([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new ApiResponseDto<object> { Success = false, Message = "Search term (q) cannot be empty." });
            }

            var searchTerm = q.ToLower(); // Case-insensitive search

            try
            {
                var products = await _context.Products
                    .Where(p => p.ProductName.ToLower().Contains(searchTerm) && p.Status == true)
                    .Take(MaxResultsPerCategory)
                    .Select(p => new ProductSearchResultDto
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        ImageUrl = p.ImageUrl // Assuming ImageUrl field exists in Products entity
                    })
                    .ToListAsync();

                var categories = await _context.Categories
                    .Where(c => c.CategoryName.ToLower().Contains(searchTerm) && c.Status == true)
                    .Take(MaxResultsPerCategory)
                    .Select(c => new CategorySearchResultDto
                    {
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName
                    })
                    .ToListAsync();

                var stores = await _context.Stores
                    .Where(s => s.StoreName.ToLower().Contains(searchTerm) && s.Status == true)
                    .Take(MaxResultsPerCategory)
                    .Select(s => new StoreSearchResultDto
                    {
                        StoreID = s.StoreID,
                        StoreName = s.StoreName
                    })
                    .ToListAsync();

                var suppliers = await _context.Suppliers
                    .Where(sup => sup.SupplierName.ToLower().Contains(searchTerm) && sup.Status == true)
                    .Take(MaxResultsPerCategory)
                    .Select(sup => new SupplierSearchResultDto
                    {
                        SupplierID = sup.SupplierID,
                        SupplierName = sup.SupplierName
                    })
                    .ToListAsync();

                var result = new GlobalSearchResultDto
                {
                    Products = products,
                    Categories = categories,
                    Stores = stores,
                    Suppliers = suppliers
                };

                return Ok(new ApiResponseDto<GlobalSearchResultDto> { Success = true, Data = result, Message = "Search completed successfully." });
            }
            catch (System.Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                // Consider logging ex.ToString() for more details including stack trace
                return StatusCode(500, new ApiResponseDto<object> { Success = false, Message = "An error occurred while processing your request.", Errors = new List<string> { ex.Message } });
            }
        }
    }
} 