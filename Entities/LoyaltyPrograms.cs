namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class LoyaltyPrograms
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int LoyaltyProgramID { get; set; }
        
        public string ProgramName { get; set; }

        public decimal DiscountRate  { get; set; }

        public int PointsMultiplier { get; set; }

        public bool Status { get; set; } = true;

        public LoyaltyPrograms() { }
    }
}
