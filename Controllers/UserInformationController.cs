using AutoMapper;
using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions require authentication
    public class UserInformationController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;

        public UserInformationController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private bool TryGetUserId(out int userId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out userId))
            {
                return true;
            }
            userId = 0;
            return false;
        }

        // GET: api/userinformation
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<UserInformationResponseDto>>> GetUserInformation()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "User ID not found in token." });
            }

            var userInfo = await _context.UserInformation
                                     .FirstOrDefaultAsync(ui => ui.UserID == userId);

            if (userInfo == null)
            {
                // Optionally, create a default one if it doesn't exist, or return not found.
                // For now, returning not found if no explicit record exists.
                return NotFound(new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "User information not found." });
            }

            var responseDto = _mapper.Map<UserInformationResponseDto>(userInfo);
            return Ok(new ApiResponseDto<UserInformationResponseDto> { Success = true, Data = responseDto });
        }

        // PUT: api/userinformation
        [HttpPut]
        public async Task<ActionResult<ApiResponseDto<UserInformationResponseDto>>> CreateOrUpdateUserInformation([FromBody] UserInformationDto userInformationDto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "User ID not found in token." });
            }

            var userInfo = await _context.UserInformation
                                     .FirstOrDefaultAsync(ui => ui.UserID == userId);

            if (userInfo == null)
            {
                // Create new UserInformation
                userInfo = _mapper.Map<UserInformation>(userInformationDto);
                userInfo.UserID = userId; // Ensure UserID is set correctly
                _context.UserInformation.Add(userInfo);
                await _context.SaveChangesAsync();
                var newResponseDto = _mapper.Map<UserInformationResponseDto>(userInfo);
                return CreatedAtAction(nameof(GetUserInformation), new { id = userInfo.UserInformationID }, new ApiResponseDto<UserInformationResponseDto> { Success = true, Data = newResponseDto, Message = "User information created successfully." });
            }
            else
            {
                // Update existing UserInformation
                _mapper.Map(userInformationDto, userInfo); // Update existing entity with DTO values
                // userInfo.UserID remains the same
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "Data conflict occurred. Please try again.", Errors = new() { ex.Message } });
                }
                var updatedResponseDto = _mapper.Map<UserInformationResponseDto>(userInfo);
                return Ok(new ApiResponseDto<UserInformationResponseDto> { Success = true, Data = updatedResponseDto, Message = "User information updated successfully." });
            }
        }
    }
} 