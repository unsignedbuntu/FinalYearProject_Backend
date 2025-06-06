﻿namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SalesDetailsDTO
    {
#nullable disable
        [Required]
        public int SaleID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int StoreID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal PriceAtSale { get; set; }
    }
}
