using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }
        
        public int UserID { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        
        public string? ShippingAddress { get; set; }
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
        
        // Collection navigation properties
        public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
} 