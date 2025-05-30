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
using Microsoft.AspNetCore.Identity;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions require authentication
    public class UserInformationController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Users> _userManager;

        public UserInformationController(
            KTUN_DbContext context,
            IMapper mapper,
            UserManager<Users> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
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
                userInfo.FirstName = userInformationDto.FirstName;
                userInfo.LastName = userInformationDto.LastName;
                userInfo.PhoneNumber = userInformationDto.PhoneNumber;
                userInfo.DateOfBirth = userInformationDto.DateOfBirth;

                // Identity Users tablosundaki kullanıcıyı da güncelle
                var identityUser = await _userManager.FindByIdAsync(userId.ToString());
                if (identityUser != null)
                {
                    identityUser.FirstName = userInformationDto.FirstName;
                    identityUser.LastName = userInformationDto.LastName;
                    identityUser.FullName = $"{userInformationDto.FirstName} {userInformationDto.LastName}";

                    var updateIdentityResult = await _userManager.UpdateAsync(identityUser);
                    if (!updateIdentityResult.Succeeded)
                    {
                        Console.WriteLine($"Error updating Identity user (Users table) for UserID {userId}: {string.Join(", ", updateIdentityResult.Errors.Select(e => e.Description))}");
                        var errorMessages = updateIdentityResult.Errors.Select(e => e.Description).ToList();
                        return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "Error updating core user profile details.", Errors = errorMessages });
                    }
                }
                else
                {
                    Console.WriteLine($"CRITICAL: User with ID {userId} exists in UserInformation but not found in Identity Users table during update.");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "Critical error: User profile cannot be found for update." });
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Concurrency hatası için loglama ve uygun yanıt
                    Console.WriteLine($"CONCURRENCY EXCEPTION while updating UserInfo for UserID {userId}: {ex.Message}");
                    return StatusCode(StatusCodes.Status409Conflict, new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "Data has been modified by another user. Please refresh and try again.", Errors = new() { ex.Message } });
                }
                catch (Exception ex) // Diğer olası SaveChanges hataları için genel catch
                {
                    Console.WriteLine($"EXCEPTION while updating UserInfo for UserID {userId}: {ex.InnerException?.Message ?? ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<UserInformationResponseDto> { Success = false, Message = "An error occurred while updating user information.", Errors = new() { ex.InnerException?.Message ?? ex.Message } });
                }

                var updatedResponseDto = _mapper.Map<UserInformationResponseDto>(userInfo);
                return Ok(new ApiResponseDto<UserInformationResponseDto> { Success = true, Data = updatedResponseDto, Message = "User information updated successfully." });
            }
        }
    }
} 