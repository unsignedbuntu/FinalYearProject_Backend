namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SalesDetailsResponseDTO
    {
#nullable disable
        public int Quantity { get; set; }

        public decimal PriceAtSale { get; set; }

        public int StoreID { get; set; }

        public int SaleID { get; set; }

        [ForeignKey("StoreID")]
        public virtual StoresDTO Stores { get; set; }

        [ForeignKey("SaleID")]

        public virtual SalesDTO Sales { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public decimal TotalAmount { get; set; }

    }
}
