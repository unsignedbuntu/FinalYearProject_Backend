namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class UserLoyaltyResponseDTO
    {
#nullable disable
        public int AccumulatedPoints { get; set; } = 0;

        public DateTime EnrollmentDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        [ForeignKey("LoyaltProgramID")]
        public int LoyaltyProgramID { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }

    }
}
