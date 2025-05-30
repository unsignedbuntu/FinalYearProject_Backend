using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.DTOs; // Assuming DTOs for Register/Login exist or will be created
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Added for DbUpdateException and KTUN_DbContext

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager; // We might not need SignInManager directly if only generating tokens
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // Optional: if using roles
        private readonly KTUN_DbContext _context; // Added KTUN_DbContext

        public AuthController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            IConfiguration configuration,
            RoleManager<IdentityRole<int>> roleManager, // Inject RoleManager if roles are needed
            KTUN_DbContext context) // Injected KTUN_DbContext
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager; // Assign RoleManager
            _context = context; // Assigned KTUN_DbContext
        }

        // POST: api/Auth/Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                // Return a more structured error response
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Validation failed.", errors });
            }

            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new { success = false, message = "User with this email already exists!" });
            }

            // 1. Create Users (Identity User)
            Users user = new Users()
            {
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerDto.Email, // Or a unique username if different from email
                FirstName = registerDto.FirstName, // Assign FirstName
                LastName = registerDto.LastName,   // Assign LastName
                // FullName can be derived or set explicitly if still needed in Users table
                FullName = $"{registerDto.FirstName} {registerDto.LastName}",
                PhoneNumber = registerDto.PhoneNumber?.Trim(), // Assign PhoneNumber from DTO
                Status = true
            };

            var identityResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (!identityResult.Succeeded)
            {
                var identityErrors = identityResult.Errors.Select(e => e.Description);
                return BadRequest(new { success = false, message = "User creation failed!", errors = identityErrors });
            }

            // 2. Create UserInformation Record
            // user.Id is populated after CreateAsync is successful
            UserInformation newUserInfo = new UserInformation
            {
                UserID = user.Id, // Foreign Key to the Users table
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber?.Trim(),
                DateOfBirth = registerDto.DateOfBirth // Assign DateOfBirth from DTO
            };

            _context.UserInformation.Add(newUserInfo);

            try
            {
                await _context.SaveChangesAsync(); // Save UserInformation to the database
            }
            catch (DbUpdateException ex)
            {
                // Log the error
                Console.WriteLine($"Error saving UserInformation for UserID {user.Id}: {ex.InnerException?.Message ?? ex.Message}");
                // Important: User (Identity) was already created. 
                // If saving UserInformation fails, you might want to:
                // 1. Inform the user that their basic account was created but profile info failed, ask to update later.
                // 2. Implement a transaction to roll back user creation if UserInfo fails (more complex).
                // For now, we'll just log and return a success for the user creation part,
                // as the primary registration (Identity user) succeeded.
                // However, it's better to signal that something went partially wrong.
                // Consider if you want to delete the created user if UserInformation saving fails.
                // For simplicity, we'll proceed but this is a point of attention for robust error handling.
                 await _userManager.DeleteAsync(user); // Attempt to delete the user if UserInformation fails
                 return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while saving user details. User registration rolled back.", error = ex.Message });
            }

            // TODO: Optionally assign roles here (e.g., await _userManager.AddToRoleAsync(user, "User");)
            // Example: await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { success = true, message = "User created successfully!" });
        }

        // POST: api/Auth/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // Check if user exists and password is correct
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                // Check if user account is active (using the custom Status property)
                if (!user.Status)
                {
                    return Unauthorized(new { Status = "Error", Message = "User account is inactive." });
                }
                
                // Generate JWT token
                var tokenString = await GenerateJwtToken(user);
                var tokenExpiry = DateTime.Now.AddHours(3); // Match token expiration

                // Token'ı HTTP-only çereze ekle
                Response.Cookies.Append("authToken", tokenString, new CookieOptions
                {
                 HttpOnly = true,
                Secure = false, // false olduğundan emin ol veya bu satırı kaldır
                 Expires = tokenExpiry,
                SameSite = SameSiteMode.Lax, // Lax kalsın
                Path = "/"
                });

                // Token yerine temel kullanıcı bilgilerini döndür (opsiyonel)
                return Ok(new
                {
                    Status = "Success",
                    Message = "Login successful.",
                    User = new // Frontend'in ihtiyaç duyabileceği bilgiler
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName
                        // İhtiyaç varsa diğer roller vs. eklenebilir
                    }
                });
            }

            // If login fails
            return Unauthorized(new { Status = "Error", Message = "Invalid email or password." });
        }

        // GET: api/Auth/Me
        [Authorize] // Bu endpoint'e erişim için geçerli bir çerez (token) gerekir
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // [Authorize] attribute'u sayesinde User.Identity'nin dolu olacağını varsayabiliriz
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                // Bu durum normalde [Authorize] nedeniyle oluşmamalı, ama bir güvenlik kontrolü
                return Unauthorized(new { Status = "Error", Message = "User not identified." });
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.Status)
            {
                // Kullanıcı bulunamadı veya pasif
                return NotFound(new { Status = "Error", Message = "User not found or inactive." });
            }

            // Kullanıcı bilgilerini döndür
            return Ok(new
            {
                Status = "Success",
                User = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    // Gerekirse roller veya diğer bilgiler eklenebilir
                }
            });
        }

        // POST: api/Auth/Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken", new CookieOptions
            {
            HttpOnly = true,
            Secure = false, // false olduğundan emin ol veya bu satırı kaldır
            SameSite = SameSiteMode.Lax, // Lax kalsın
            Path = "/"
            });
            return Ok(new { Status = "Success", Message = "Logged out successfully." });
        }

        // Helper method for JWT generation
        private async Task<string> GenerateJwtToken(Users user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("fullName", user.FullName ?? "")
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] 
                ?? throw new InvalidOperationException("JWT Secret not found in configuration.")));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 