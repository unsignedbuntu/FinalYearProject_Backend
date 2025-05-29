using AutoMapper;
using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions require authentication
    public class AddressesController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;

        public AddressesController(KTUN_DbContext context, IMapper mapper)
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

        // GET: api/addresses
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<UserAddressResponseDto>>>> GetUserAddresses()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<List<UserAddressResponseDto>> { Success = false, Message = "User ID not found in token." });
            }

            var addresses = await _context.UserAddresses
                                        .Where(ua => ua.UserID == userId && ua.Status == true) // Only active addresses
                                        .OrderByDescending(ua => ua.IsDefault) // Default address first
                                        .ThenByDescending(ua => ua.UserAddressID) // Then by latest
                                        .ToListAsync();

            var responseDto = _mapper.Map<List<UserAddressResponseDto>>(addresses);
            return Ok(new ApiResponseDto<List<UserAddressResponseDto>> { Success = true, Data = responseDto });
        }

        // GET: api/addresses/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserAddressResponseDto>>> GetUserAddress(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "User ID not found in token." });
            }

            var address = await _context.UserAddresses
                                      .FirstOrDefaultAsync(ua => ua.UserAddressID == id && ua.UserID == userId);

            if (address == null)
            {
                return NotFound(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "Address not found or you do not have permission to access it." });
            }

            if (!address.Status) // Optionally, decide if inactive addresses can be fetched by ID
            {
                return NotFound(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "Address is not active." });
            }

            var responseDto = _mapper.Map<UserAddressResponseDto>(address);
            return Ok(new ApiResponseDto<UserAddressResponseDto> { Success = true, Data = responseDto });
        }

        // POST: api/addresses
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserAddressResponseDto>>> CreateUserAddress([FromBody] UserAddressDto userAddressDto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "User ID not found in token." });
            }

            var newAddress = _mapper.Map<UserAddress>(userAddressDto);
            newAddress.UserID = userId;
            newAddress.Status = true; // New addresses are active by default

            // If this new address is set to be default, unset other default addresses for this user
            if (newAddress.IsDefault)
            {
                var currentDefaultAddresses = await _context.UserAddresses
                                                          .Where(ua => ua.UserID == userId && ua.IsDefault)
                                                          .ToListAsync();
                foreach (var addr in currentDefaultAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            _context.UserAddresses.Add(newAddress);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<UserAddressResponseDto>(newAddress);
            return CreatedAtAction(nameof(GetUserAddress), new { id = newAddress.UserAddressID }, new ApiResponseDto<UserAddressResponseDto> { Success = true, Data = responseDto, Message = "Address created successfully." });
        }

        // PUT: api/addresses/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserAddressResponseDto>>> UpdateUserAddress(int id, [FromBody] UserAddressDto userAddressDto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "User ID not found in token." });
            }

            if (id != userAddressDto.UserAddressID && userAddressDto.UserAddressID != 0) // Allow UserAddressID to be 0 in DTO if not specifying it
            {
                return BadRequest(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "Address ID in URL does not match Address ID in body." });
            }

            var addressToUpdate = await _context.UserAddresses
                                            .FirstOrDefaultAsync(ua => ua.UserAddressID == id && ua.UserID == userId);

            if (addressToUpdate == null)
            {
                return NotFound(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "Address not found or you do not have permission to update it." });
            }

            if (!addressToUpdate.Status) // Do not allow updates on inactive addresses, or re-activate it if needed.
            {
                 return BadRequest(new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "Cannot update an inactive address. Please activate it first or contact support." });
            }

            _mapper.Map(userAddressDto, addressToUpdate);
            addressToUpdate.UserID = userId; // Ensure UserID is set correctly AFTER mapping to prevent overwrite if DTO had UserID

            // If this address is being set to default, unset other default addresses for this user
            if (addressToUpdate.IsDefault)
            {
                var currentDefaultAddresses = await _context.UserAddresses
                                                          .Where(ua => ua.UserID == userId && ua.IsDefault && ua.UserAddressID != addressToUpdate.UserAddressID)
                                                          .ToListAsync();
                foreach (var addr in currentDefaultAddresses)
                {
                    addr.IsDefault = false;
                }
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ApiResponseDto<UserAddressResponseDto> { Success = false, Message = "Data conflict occurred. Please try again.", Errors = new() { ex.Message } });
            }

            var updatedResponseDto = _mapper.Map<UserAddressResponseDto>(addressToUpdate);
            return Ok(new ApiResponseDto<UserAddressResponseDto> { Success = true, Data = updatedResponseDto, Message = "Address updated successfully." });
        }

        // DELETE: api/addresses/{id} (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteUserAddress(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(new ApiResponseDto<object> { Success = false, Message = "User ID not found in token." });
            }

            var addressToDelete = await _context.UserAddresses
                                            .FirstOrDefaultAsync(ua => ua.UserAddressID == id && ua.UserID == userId);

            if (addressToDelete == null)
            {
                return NotFound(new ApiResponseDto<object> { Success = false, Message = "Address not found or you do not have permission to delete it." });
            }

            if (!addressToDelete.Status) // Already inactive
            {
                return Ok(new ApiResponseDto<object> { Success = true, Message = "Address is already inactive." });
            }

            addressToDelete.Status = false;
            if(addressToDelete.IsDefault)
            {
                addressToDelete.IsDefault = false; // Cannot have an inactive default address
                // Optionally, set another address as default if this was the only default one.
                // For simplicity, we are just unsetting it. The user might need to pick a new default.
            }

            await _context.SaveChangesAsync();
            return Ok(new ApiResponseDto<object> { Success = true, Message = "Address marked as inactive successfully." });
        }
    }
} 