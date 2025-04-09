using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        public int StoreID { get; set; }
        
        [Required]
        public int CategoryID { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        public int StockQuantity { get; set; }
        
        [MaxLength(50)]
        public string? Barcode { get; set; }
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("StoreID")]
        public virtual Stores? Store { get; set; }
        
        [ForeignKey("CategoryID")]
        public virtual Categories? Category { get; set; }
        
        // Collection navigation properties
        public virtual ICollection<ProductSuppliers> ProductSuppliers { get; set; } = new List<ProductSuppliers>();
        public virtual ICollection<Inventory> InventoryRecords { get; set; } = new List<Inventory>();
        public virtual ICollection<SalesDetails> SalesDetails { get; set; } = new List<SalesDetails>();
        public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public virtual ICollection<ProductRecommendations> ProductRecommendations { get; set; } = new List<ProductRecommendations>();
    }
} 