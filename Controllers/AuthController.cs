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

        public AuthController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            IConfiguration configuration,
            RoleManager<IdentityRole<int>> roleManager) // Inject RoleManager if roles are needed
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager; // Assign RoleManager
        }

        // POST: api/Auth/Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if model state is valid based on DTO attributes
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new { Status = "Error", Message = "User with this email already exists!" });
            }

            // Create the user object
            Users user = new Users()
            {
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(), // Necessary for Identity
                UserName = registerDto.Email, // Use email as username for simplicity
                FullName = $"{registerDto.FirstName} {registerDto.LastName}",
                PhoneNumber = registerDto.PhoneNumber,
                Status = true // Default status
            };

            // Create the user in the database
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                // Collect errors
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Status = "Error", Message = "User creation failed! Please check user details and try again.", Errors = errors });
            }

            // TODO: Optionally assign roles here (e.g., await _userManager.AddToRoleAsync(user, "User");)

            return Ok(new { Status = "Success", Message = "User created successfully!" });
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