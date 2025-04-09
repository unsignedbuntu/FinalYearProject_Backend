namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ProductsDTO
    {
#nullable disable
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        
        [Required]
        public int StoreID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [MaxLength(50)]
        public string Barcode { get; set; }
    }
}
