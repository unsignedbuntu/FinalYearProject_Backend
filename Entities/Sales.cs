using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Sales
    {
        [Key]
        public int SaleID { get; set; }
        
        public int UserID { get; set; }
        
        public int StoreID { get; set; }
        
        public DateTime SaleDate { get; set; } = DateTime.Now;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
        
        [ForeignKey("StoreID")]
        public virtual Stores? Store { get; set; }
        
        // Collection navigation properties
        public virtual ICollection<SalesDetails> SalesDetails { get; set; } = new List<SalesDetails>();
    }
} 