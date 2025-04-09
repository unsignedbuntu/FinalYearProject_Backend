namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserLoyaltyDTO
    {
#nullable disable
        [Required]
        public int UserID { get; set; }

        [Required]
        public int LoyaltyProgramID { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AccumulatedPoints { get; set; } = 0;
    }
}
