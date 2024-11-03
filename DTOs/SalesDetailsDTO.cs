namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SalesDetailsDTO
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int SaleDetailID { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtSale { get; set; }

        public bool Status { get; set; } = true;

        public virtual StoresDTO Stores { get; set; }

        public virtual SalesDTO Sales { get; set; }
    }
}
