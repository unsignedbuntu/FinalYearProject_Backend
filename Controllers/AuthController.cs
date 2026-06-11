using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.DTOs;
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
using Microsoft.EntityFrameworkCore;

namespace KTUN_Final_Year_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly KTUN_DbContext _context;

        public AuthController( UserManager<Users> userManager, SignInManager<Users> signInManager, IConfiguration configuration, RoleManager<IdentityRole<int>> roleManager, KTUN_DbContext context )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }

        // POST: api/Auth/Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
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
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                FullName = $"{registerDto.FirstName} {registerDto.LastName}",
                PhoneNumber = registerDto.PhoneNumber?.Trim(),
                Status = true
            };

            var identityResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (!identityResult.Succeeded)
            {
                var identityErrors = identityResult.Errors.Select(e => e.Description);
                return BadRequest(new { success = false, message = "User creation failed!", errors = identityErrors });
            }

            // 2. Create UserInformation Record
            UserInformation newUserInfo = new UserInformation
            {
                UserID = user.Id,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber?.Trim(),
                DateOfBirth = registerDto.DateOfBirth
            };

            _context.UserInformation.Add(newUserInfo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving UserInformation for UserID {user.Id}: {ex.InnerException?.Message ?? ex.Message}");
                await _userManager.DeleteAsync(user);
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while saving user details. User registration rolled back.", error = ex.Message });
            }

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

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                if (!user.Status)
                {
                    return Unauthorized(new { Status = "Error", Message = "User account is inactive." });
                }

                var tokenString = await GenerateJwtToken(user);

                // Frontend JSON'dan alıp kendi çerezine (cookie) ekleyeceği için 
                // Token ve User bilgilerini doğrudan gönderiyoruz.
                return Ok(new
                {
                    Status = "Success",
                    Message = "Login successful.",
                    Token = tokenString,
                    User = new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName
                    }
                });
            }

            return Unauthorized(new { Status = "Error", Message = "Invalid email or password." });
        }

        // GET: api/Auth/Me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Status = "Error", Message = "User not identified." });
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.Status)
            {
                return NotFound(new { Status = "Error", Message = "User not found or inactive." });
            }

            return Ok(new
            {
                Status = "Success",
                User = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                }
            });
        }

        // POST: api/Auth/Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Backend'in cookie silmesine gerek kalmadı. 
            // Frontend tarafında çıkış yaparken Cookies.remove('authToken') komutu kullanılacak.
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
        // POST: api/Auth/change-password
        [Authorize] // Sadece giriş yapmış (token'ı olan) kullanıcılar şifre değiştirebilir!
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            // 1. Token'dan (Yaka kartından) isteği atan kişinin kimliğini (ID) bul
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(new { success = false, message = "User not identified." });
            }

            // 2. Veritabanından o kullanıcıyı getir
            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            // 3. Identity kütüphanesini kullanarak şifreyi güvenli bir şekilde değiştir
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Password updated successfully!" });
            }

            // 4. Eğer eski şifre yanlışsa veya yeni şifre kurallara uymuyorsa hata dön
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { success = false, message = "Failed to update password.", errors = errors });
        }
    }
}