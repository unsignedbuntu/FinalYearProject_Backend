using System;

namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class UserInformationResponseDto
    {
        public int UserInformationID { get; set; }
        public int UserID { get; set; } // To know which user this info belongs to
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        // You can add more user-related details here if needed from the Users entity during mapping
        // public string UserEmail { get; set; }
    }
} 