using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class ProductSuppliers
    {
        [Key]
        public int ProductSupplierID { get; set; }
        
        public int ProductID { get; set; }
        
        public int SupplierID { get; set; }
        
        public DateTime SupplyDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; }
        
        [ForeignKey("SupplierID")]
        public virtual Suppliers? Supplier { get; set; }
    }
} 