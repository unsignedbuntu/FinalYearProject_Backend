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


        [ForeignKey("StoreID")]
        public int StoreID { get; set; }

        [ForeignKey("SaleID")]

        public int SaleID { get; set; }

    }
}
