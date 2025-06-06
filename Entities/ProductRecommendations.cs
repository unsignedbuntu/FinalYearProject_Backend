using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class ProductRecommendations
    {
        [Key]
        public int RecommendationID { get; set; }
        
        public int UserID { get; set; }
        
        public int ProductID { get; set; }
        
        public DateTime RecommendationDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
        
        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; }
    }
} 