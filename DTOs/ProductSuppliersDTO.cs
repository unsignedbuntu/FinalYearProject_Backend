namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ProductSuppliersDTO
    {
#nullable disable
        [Required]
        public int ProductID { get; set; }

        [Required]
        public int SupplierID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Satın alma fiyatı 0'dan büyük olmalıdır.")]
        public decimal PurchasePrice { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Quantity { get; set; }
    }
}
