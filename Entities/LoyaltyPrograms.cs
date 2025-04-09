using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class LoyaltyPrograms
    {
        [Key]
        public int LoyaltyProgramID { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ProgramName { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountRate { get; set; }
        
        public int PointsMultiplier { get; set; }
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<UserLoyalty> UserLoyaltyEntries { get; set; } = new List<UserLoyalty>();
    }
} 