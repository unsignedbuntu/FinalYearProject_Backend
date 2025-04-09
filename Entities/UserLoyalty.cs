namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
    public class UserLoyalty
    {
        [Key]
        public int UserLoyaltyID { get; set; }

        public int UserID { get; set; }

        public int LoyaltyProgramID { get; set; }

        public int AccumulatedPoints { get; set; } = 0;

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }

        [ForeignKey("LoyaltyProgramID")]
        public virtual LoyaltyPrograms? LoyaltyProgram { get; set; }

        public UserLoyalty() { }
    }
#nullable disable
}
