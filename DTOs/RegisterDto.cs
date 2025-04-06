using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = default!;

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = default!;

        // Optional for now, can be added to Users entity later if needed
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        // Consider adding complexity requirements validation later
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = default!;

        // We can add Day, Month, Year properties here if needed later
    }
} 