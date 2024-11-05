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
    public class ProductRecommandationsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public ProductRecommandationsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetProductRecommandations()
        {
            var recommandations = _context.ProductRecommendations
                .Include(pr => pr.User)
                .Include(pr => pr.Product)
                .Where(pr => pr.Status == true)
                .Select(pr => _mapper.Map<ProductRecommendationsDTO>(pr))
                .ToList();

            return Ok(recommandations);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetProductRecommandationsByID(int id)
        {
            var recommandations = _context.ProductRecommendations
                .Include(pr => pr.User)
                .Include(pr => pr.Product)
                .Where(pr => pr.RecommendationID == id)
                .Where(pr => pr.Status == true)
                .Select(pr => _mapper.Map<ProductRecommendationsDTO>(pr))
                .FirstOrDefault();

            if (recommandations == null)
            {
                return NotFound();
            }

            return Ok(recommandations);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateProductRecommandations([FromBody] ProductRecommendationsResponseDTO productRecommendationsResponseDTO)
        {
            if ( productRecommendationsResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productRecommandationsResponse = _mapper.Map<ProductRecommendations>(productRecommendationsResponseDTO);

            _context.ProductRecommendations.Add(productRecommandationsResponse);
            _context.SaveChanges();

            return Ok(productRecommandationsResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateProductRecommandations(int id, [FromBody] ProductRecommendationsResponseDTO productRecommendationsResponseDTO)
        {
            if (productRecommendationsResponseDTO == null)
            {
                return BadRequest();
            }

            var productRecommandationsResponse = _context.ProductRecommendations.FirstOrDefault(pr => pr.RecommendationID == id);

            if (productRecommandationsResponse == null)
            {
                return NotFound();
            }

            productRecommandationsResponse.RecommendationDate = productRecommendationsResponseDTO.RecommendationDate;


            _context.SaveChanges();

            return Ok(productRecommandationsResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteProductRecommandationsByStatus(int id)
        {
            var recommandations = _context.ProductRecommendations.FirstOrDefault(pr => pr.RecommendationID == id);

            if (recommandations == null)
            {
                return NotFound();
            }


            recommandations.Status = false;


            _context.ProductRecommendations.Update(recommandations);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

