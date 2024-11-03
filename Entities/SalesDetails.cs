namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SalesDetails
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int SaleDetailID { get; set; }

        public int SaleID {  get; set; }

        public int StoreID { get; set; }

        [ForeignKey("SaleID")]

        public virtual Sales Sale { get; set; }

        [ForeignKey("StoreID")]

        public virtual Stores Store { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtSale { get; set; }

        public bool Status { get; set; } = true;

        public SalesDetails() { }
    }
}
