using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportMessagesController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        
        public SupportMessagesController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetSupportMessages()
        {
            var messages = _context.SupportMessages
                .Include(m => m.User)
                .Where(m => m.Status != "Deleted")
                .ToList();

            return Ok(messages);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetSupportMessageByID(int id)
        {
            var message = _context.SupportMessages
                .Include(m => m.User)
                .Where(m => m.MessageID == id)
                .Where(m => m.Status != "Deleted")
                .FirstOrDefault();

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        [HttpGet("User/{userId}")]
        [Produces("application/json")]
        public IActionResult GetMessagesByUser(int userId)
        {
            var messages = _context.SupportMessages
                .Include(m => m.User)
                .Where(m => m.UserID == userId && m.Status != "Deleted")
                .ToList();

            return Ok(messages);
        }

        [HttpGet("Open")]
        [Produces("application/json")]
        public IActionResult GetOpenMessages()
        {
            var messages = _context.SupportMessages
                .Include(m => m.User)
                .Where(m => m.Status == "Open" || m.Status == null)
                .ToList();

            return Ok(messages);
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateSupportMessage([FromBody] SupportMessagesDTO messageDTO)
        {
            if (messageDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kullanıcı kontrolü
            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(messageDTO.UserID));
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            var supportMessage = _mapper.Map<SupportMessages>(messageDTO);
            supportMessage.Timestamp = DateTime.Now;
            supportMessage.Status = "Open";

            _context.SupportMessages.Add(supportMessage);
            _context.SaveChanges();

            var createdMessage = _context.SupportMessages
                .Include(m => m.User)
                .FirstOrDefault(m => m.MessageID == supportMessage.MessageID);

            return Ok(createdMessage);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateSupportMessage(int id, [FromBody] SupportMessageDTO messageDTO)
        {
            if (messageDTO == null)
            {
                return BadRequest();
            }

            var message = _context.SupportMessages.FirstOrDefault(m => m.MessageID == id && m.Status != "Deleted");
            if (message == null)
            {
                return NotFound();
            }

            // Sadece mesaj sahibi güncelleyebilir
            if (message.UserID != messageDTO.UserID)
            {
                return Forbid();
            }

            message.Subject = messageDTO.Subject;
            message.Message = messageDTO.MessageContent;
            message.Timestamp = DateTime.Now;

            _context.SupportMessages.Update(message);
            _context.SaveChanges();

            var updatedMessage = _context.SupportMessages
                .Include(m => m.User)
                .FirstOrDefault(m => m.MessageID == id);
            
            var response = _mapper.Map<SupportMessagesResponseDTO>(updatedMessage);

            return Ok(response);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteMessageByStatus(int id)
        {
            var message = _context.SupportMessages.FirstOrDefault(m => m.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            message.Status = "Deleted";

            _context.SupportMessages.Update(message);
            _context.SaveChanges();

            return NoContent();
        }
    }
} 