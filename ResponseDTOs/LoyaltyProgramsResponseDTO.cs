namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public class LoyaltyProgramsResponseDTO
    {
        public int LoyaltyProgramID { get; set; }
        
        public string ProgramName { get; set; } = string.Empty;
                
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountRate { get; set; }
        
        public int PointsMultiplier { get; set; }
        
        public bool Status { get; set; }

        // İstatistikler
        public int EnrolledUserCount { get; set; }
    }
}
