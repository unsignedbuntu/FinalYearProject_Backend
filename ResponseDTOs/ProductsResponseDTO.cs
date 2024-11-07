namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductsResponseDTO
    {
#nullable disable

        public string ProductName { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Barcode { get; set; }

        [ForeignKey("StoreID")]

        public int StoreID { get; set; }

        [ForeignKey("CategoryID")]

        public int CategoryID { get; set; }
    }
}
