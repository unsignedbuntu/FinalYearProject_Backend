using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.Entities
{
    public class Suppliers
    {
        [Key]
        public int SupplierID { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? ContactEmail { get; set; }
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<ProductSuppliers> ProductSuppliers { get; set; } = new List<ProductSuppliers>();
    }
} 