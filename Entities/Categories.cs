using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Categories
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        public int StoreID { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
                
        public bool Status { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("StoreID")]
        public virtual Stores? Store { get; set; }
                
        // Collection navigation properties
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
        // SubCategories kaldırıldı çünkü DB'de ParentCategoryID yok.
    }
} 