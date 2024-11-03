namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Products
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int StoreID { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey("StoreID")]

        public virtual Stores Store { get; set; }

        [ForeignKey("CategoryID")]
        
        public virtual Categories Category { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Barcode { get; set; }

        public bool Status { get; set; } = true;

        public Products() { }

    }
}
