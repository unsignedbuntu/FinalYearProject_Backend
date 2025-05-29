namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class UserAddressResponseDto
    {
        public int UserAddressID { get; set; }
        public int UserID { get; set; } // To know which user this address belongs to
        public string? AddressTitle { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? FullAddress { get; set; }
        public bool IsDefault { get; set; }
        public bool Status { get; set; } // Include status in response
        // You can add more user-related details here if needed from the Users entity during mapping
        // public string UserEmail { get; set; }
    }
} 