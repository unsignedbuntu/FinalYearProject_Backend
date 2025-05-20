using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class ImageCache
    {
        [Key]
        public int ID { get; set; }
        
        public string? Prompt { get; set; }
        
        public byte[]? Image { get; set; }
        
        [MaxLength(64)]
        public string? HashValue { get; set; }
        
        public bool Status { get; set; } = true;

        // Foreign Key for Products
        public int? ProductID { get; set; } // Nullable FK

        // Navigation property
        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; }

        // Foreign Key for Suppliers
        public int? SupplierID { get; set; } // Nullable FK

        // Navigation property
        [ForeignKey("SupplierID")]
        public virtual Suppliers? Supplier { get; set; }
    }
}