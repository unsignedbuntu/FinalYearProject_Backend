using System;
using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class UserInformationDto
    {
        // UserID will likely be inferred from the authenticated user context, not directly passed in DTO for create/update
        // For GET, UserID might be included if an admin is fetching it, but for personal info, it's implicit.
        public int UserInformationID { get; set; } // For returning existing record ID

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? PhoneNumber { get; set; }
    }
} 