using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class UserAddressDto
    {
        public int UserAddressID { get; set; } // For returning existing record ID, or for update/delete

        [Required(ErrorMessage = "Address title is required.")]
        [StringLength(100)]
        public string AddressTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name for the address is required.")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number for the address is required.")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(100)]
        public string? District { get; set; }

        [Required(ErrorMessage = "Full address is required.")]
        [StringLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
        // Status is typically managed by the backend, not set by client for create/update directly
        // public bool Status { get; set; } = true; 
    }
} 