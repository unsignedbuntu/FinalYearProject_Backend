namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class UserLoyalty
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int UserLoyaltyID { get; set; }

        public int UserID { get; set; }

        public int LoyaltyProgramID { get; set; }

        [ForeignKey("UserID")]

        public virtual Users User { get; set; }

        [ForeignKey("LoyaltyProgramID")]

        public virtual LoyaltyPrograms LoyaltyProgram { get; set; }

        public int AccumulatedPoints { get; set; } = 0;

        public DateTime EnrollmentDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;

        public UserLoyalty() { }
    }
}
