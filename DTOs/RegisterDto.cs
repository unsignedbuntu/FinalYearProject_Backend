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

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = default!; // Bu alan DTO'da kalmalý ve frontend göndermeli

        public DateTime? DateOfBirth { get; set; }
    }
}