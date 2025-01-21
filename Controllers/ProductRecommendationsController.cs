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
    public class ProductRecommendationsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public ProductRecommendationsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetProductRecommendations()
        {
            var recommendations = _context.ProductRecommendations
                .Include(pr => pr.User)
                .Include(pr => pr.Product)
                .Where(pr => pr.Status == true)
                .Select(pr => _mapper.Map<ProductRecommendations>(pr))
                .ToList();

            return Ok(recommendations);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetProductRecommendationsByID(int id)
        {
            var recommendations = _context.ProductRecommendations
                .Include(pr => pr.User)
                .Include(pr => pr.Product)
                .Where(pr => pr.RecommendationID == id)
                .Where(pr => pr.Status == true)
                .Select(pr => _mapper.Map<ProductRecommendations>(pr))
                .FirstOrDefault();

            if (recommendations == null)
            {
                return NotFound();
            }

            return Ok(recommendations);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateProductRecommendations([FromBody] ProductRecommendationsResponseDTO productRecommendationsResponseDTO)
        {
            if ( productRecommendationsResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productRecommendationsResponse = _mapper.Map<ProductRecommendations>(productRecommendationsResponseDTO);

            _context.ProductRecommendations.Add(productRecommendationsResponse);
            _context.SaveChanges();

            return Ok(productRecommendationsResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateProductRecommendations(int id, [FromBody] ProductRecommendationsResponseDTO productRecommendationsResponseDTO)
        {
            if (productRecommendationsResponseDTO == null)
            {
                return BadRequest();
            }

            var productRecommendationsResponse = _context.ProductRecommendations.FirstOrDefault(pr => pr.RecommendationID == id);

            if (productRecommendationsResponse == null)
            {
                return NotFound();
            }

            productRecommendationsResponse.RecommendationDate = productRecommendationsResponseDTO.RecommendationDate;

            productRecommendationsResponse.UserID = productRecommendationsResponseDTO.UserID;

            productRecommendationsResponse.ProductID = productRecommendationsResponseDTO.ProductID;

            _context.SaveChanges();

            return Ok(productRecommendationsResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteProductRecommendationsByStatus(int id)
        {
            var recommendations = _context.ProductRecommendations.FirstOrDefault(pr => pr.RecommendationID == id);

            if (recommendations == null)
            {
                return NotFound();
            }


            recommendations.Status = false;


            _context.ProductRecommendations.Update(recommendations);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

