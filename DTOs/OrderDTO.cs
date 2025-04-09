using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string UserID { get; set; } = default!;

        public int? StoreID { get; set; }

        [Required]
        [StringLength(4000)]
        public string DeliveryAddress { get; set; } = default!;

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        // Sipariş kalemleri
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }

    public class OrderItemDTO
    {
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Quantity { get; set; }
    }

    public class OrderUpdateDTO
    {
        [StringLength(50)]
        public string OrderStatus { get; set; } = default!;

        [StringLength(4000)]
        public string? ShippingAddress { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }
    }
} 