using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Identity.Client;

namespace KTUN_Final_Year_Project.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ImageCacheController : ControllerBase
    {

        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        public ImageCacheController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{pageID}/{prompt}")]
        [Produces("application/json")]
        public IActionResult GetImageCache(string pageID, string prompt)
        {
            if (string.IsNullOrEmpty(pageID) || string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Missing required parameters");
            }

            string hashInput = prompt;
            string hashValue = GenerateHash(hashInput);


            try
            {
                var imageCacheEntity = _context.ImageCache.FirstOrDefault(ic => ic.PageID == pageID && ic.HashValue == hashValue);
                if (imageCacheEntity == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    cached = true,
                    image = Convert.ToBase64String(imageCacheEntity.Image)
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetImageCacheByID(int id)
        {
            var imageCache = _context.ImageCache
                .Where (ic => ic.ID == id)
                .Where(ic => ic.Status == true)
                .Select(ic => _mapper.Map<ImageCache>(ic))
                .FirstOrDefault();

            if (imageCache == null)
            {
                return NotFound();
            }

            return Ok(imageCache);
        }

        [HttpPost]
        [Produces("application/json")]
         public IActionResult CreateImageCache([FromBody] ImageCacheResponseDTO imageCacheResponseDTO)
        {
            if (imageCacheResponseDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            string hashInput = imageCacheResponseDTO.Prompt;

            string hashValue = GenerateHash(hashInput);

            var existingImageCache = _context.ImageCache.FirstOrDefault(ic => ic.HashValue == hashValue);


            if (existingImageCache != null)
            {
                return Ok(new
                {
                    image = Convert.ToBase64String(existingImageCache.Image)
                });
            }


            var imageCacheEntity = _mapper.Map<ImageCache>(imageCacheResponseDTO);
            imageCacheEntity.HashValue = hashValue;

            _context.ImageCache.Add(imageCacheEntity);
            _context.SaveChanges();

            return Ok(new
            {
                image = Convert.ToBase64String(imageCacheEntity.Image)
            });
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        
        public IActionResult UpdateImageCache(int id, [FromBody] ImageCacheResponseDTO imageCacheResponseDTO)
        {
            if (imageCacheResponseDTO == null)
            {
                return BadRequest();
            }

            var imageCacheResponse = _context.ImageCache.FirstOrDefault(ic => ic.ID == id);

            if (imageCacheResponse == null)
            {
                return NotFound();
            }
            imageCacheResponse.PageID = imageCacheResponse.PageID;
            imageCacheResponse.Prompt = imageCacheResponse.Prompt;
            imageCacheResponse.Image = imageCacheResponse.Image;
            imageCacheResponse.Status = imageCacheResponse.Status;

            _context.SaveChanges();

            return Ok(imageCacheResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteImageCacheByStatus(int id)
        {
            var imageCache = _context.ImageCache.FirstOrDefault(ic => ic.ID == id);

            if (imageCache == null)
            {
                return NotFound();
            }

            imageCache.Status = false;
            _context.SaveChanges();

            return Ok(imageCache);
        }

        private string GenerateHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return ""; // Boş bir hash döndürebilir veya bir default değer
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
