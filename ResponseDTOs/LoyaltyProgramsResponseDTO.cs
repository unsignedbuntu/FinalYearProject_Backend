namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class LoyaltyProgramsResponseDTO
    {
#nullable disable
        public string ProgramName { get; set; }

        public decimal DiscountRate { get; set; }

        public int PointsMultiplier { get; set; }
    }
}
