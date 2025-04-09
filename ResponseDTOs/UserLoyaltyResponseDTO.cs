namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class UserLoyaltyResponseDTO
    {
        public int UserLoyaltyID { get; set; }
        
        public int UserID { get; set; }
        
        public int LoyaltyProgramID { get; set; }
        
        public int AccumulatedPoints { get; set; }
        
        public DateTime EnrollmentDate { get; set; }
        
        public bool Status { get; set; }
        
        // Kullanıcı ve program bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
    }
}
