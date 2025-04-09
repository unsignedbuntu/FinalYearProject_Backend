using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Stores
    {
        [Key]
        public int StoreID { get; set; }

        [Required]
        [MaxLength(100)]
        public string StoreName { get; set; } = string.Empty;
        
        public bool Status { get; set; } = true;

        // Navigation properties (Relationships defined in DbContext or other entities)
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
        public virtual ICollection<Categories> Categories { get; set; } = new List<Categories>();
        public virtual ICollection<Sales> Sales { get; set; } = new List<Sales>();
        public virtual ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
        public virtual ICollection<SalesDetails> SalesDetails { get; set; } = new List<SalesDetails>();
    }
} 