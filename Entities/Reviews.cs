using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Reviews
    {
        [Key]
        public int ReviewID { get; set; }
        
        public int UserID { get; set; }
        
        public int ProductID { get; set; }
        
        public int? OrderItemID { get; set; }
        
        public int Rating { get; set; }
        
        public string? Comment { get; set; }
        
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
        
        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; }

        [ForeignKey("OrderItemID")]
        public virtual OrderItems? OrderItem { get; set; }
    }
} 