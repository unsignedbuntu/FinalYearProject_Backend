namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class UserLoyaltyResponseDTO
    {
#nullable disable

        public int LoyaltyProgramsID { get; set; }

        public int UserID { get; set; }

        [ForeignKey("LoyaltProgramsID")]
        public virtual LoyaltyProgramsDTO LoyaltyPrograms { get; set; }

        [ForeignKey("UserID")]
        public virtual UsersDTO Users { get; set; }
        
        public int AccumulatedPoints { get; set; } = 0;

        public DateTime EnrollmentDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }
}
