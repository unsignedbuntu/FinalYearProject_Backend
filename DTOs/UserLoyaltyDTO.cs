namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class UserLoyaltyDTO
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int UserLoyaltyID { get; set; }

        public int AccumulatedPoints { get; set; } = 0;

        public DateTime EnrollmentDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;

        public virtual LoyaltyProgramsDTO LoyaltyPrograms { get; set; }

        public virtual UsersDTO Users { get; set; }
    }
}
