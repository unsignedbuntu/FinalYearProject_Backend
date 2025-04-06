using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
} 