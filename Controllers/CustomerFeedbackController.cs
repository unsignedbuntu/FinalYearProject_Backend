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
    public class CustomerFeedbackController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public CustomerFeedbackController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetCustomerFeedback()
        {
            var customerFeedback = _context.CustomerFeedback
                .Include(cf => cf.User)
                .Include(cf => cf.Product)  
                .Where(cf => cf.Status == true)
                .Select(cf => _mapper.Map<CustomerFeedback>(cf))
                .ToList();

            return Ok(customerFeedback);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetCustomerFeedbackByID(int id)
        {
            var customerFeedback = _context.CustomerFeedback
                .Include(cf => cf.User)
                .Include(cf => cf.Product)
                .Where(cf => cf.CustomerFeedbackID == id)
                .Where(cf => cf.Status == true)
                .Select(cf => _mapper.Map<CustomerFeedback>(cf))
                .FirstOrDefault();

            if (customerFeedback == null)
            {
                return NotFound();
            }

            return Ok(customerFeedback);
        }


        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateCustomerFeedback([FromBody] CustomerFeedbackResponseDTO customerFeedbackResponseDTO)
        {
            if (customerFeedbackResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerFeedbackResponse = _mapper.Map<CustomerFeedback>(customerFeedbackResponseDTO);

            _context.CustomerFeedback.Add(customerFeedbackResponse);
            _context.SaveChanges();

            return Ok(customerFeedbackResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateCustomerFeedback(int id, [FromBody] CustomerFeedbackResponseDTO customerFeedbackResponseDTO)
        {
            if (customerFeedbackResponseDTO == null)
            {
                return BadRequest();
            }

            var customerFeedbackResponse = _context.CustomerFeedback.FirstOrDefault(cf => cf.CustomerFeedbackID == id);

            if (customerFeedbackResponse == null)
            {
                return NotFound();
            }

            customerFeedbackResponse.FeedbackText= customerFeedbackResponseDTO.FeedbackText;

            customerFeedbackResponse.Rating = customerFeedbackResponseDTO.Rating;
            
            customerFeedbackResponse.FeedbackDate = customerFeedbackResponseDTO.FeedbackDate;

            customerFeedbackResponse.UserID = customerFeedbackResponseDTO.UserID;

            customerFeedbackResponse.ProductID = customerFeedbackResponseDTO.ProductID;


            _context.SaveChanges();

            return Ok(customerFeedbackResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteCustomerFeedbackByStatus(int id)
        {
            var customerFeedback = _context.CustomerFeedback.FirstOrDefault(cf => cf.CustomerFeedbackID == id);

            if (customerFeedback == null)
            {
                return NotFound();
            }


            customerFeedback.Status = false;


            _context.CustomerFeedback.Update(customerFeedback);
            _context.SaveChanges();

            return NoContent();
        }

    }
}

