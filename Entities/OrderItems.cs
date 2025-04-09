using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class OrderItems
    {
        [Key]
        public int OrderItemID { get; set; }
        
        public int OrderID { get; set; }
        
        public int ProductID { get; set; }
        
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }
        
        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual Orders? Order { get; set; }
        
        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; }
    }
} 