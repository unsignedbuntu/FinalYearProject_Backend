namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrdersDTO
    {
#nullable disable
        [Required]
        public int UserID { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0'dan büyük olmalıdır.")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(4000)]
        public string ShippingAddress { get; set; }
    }
} 