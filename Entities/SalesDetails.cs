using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class SalesDetails
    {
        [Key]
        public int SaleDetailID { get; set; }
        
        public int SaleID { get; set; }
        
        public int StoreID { get; set; }
        
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtSale { get; set; }
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("SaleID")]
        public virtual Sales? Sale { get; set; }
        
        [ForeignKey("StoreID")]
        public virtual Stores? Store { get; set; }
    }
}
