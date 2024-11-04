namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductsResponseDTO
    {
#nullable disable
        public int StoreID { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey("StoreID")]

        public virtual StoresDTO Stores { get; set; }

        [ForeignKey("CategoryID")]

        public virtual CategoriesDTO Categories { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Barcode { get; set; }

    }
}
