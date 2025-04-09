using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }
        
        public int ProductID { get; set; }
        
        [MaxLength(50)]
        public string? ChangeType { get; set; }
        
        public int QuantityChanged { get; set; }
        
        public DateTime ChangeDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Products Product { get; set; } = null!;
    }
}
