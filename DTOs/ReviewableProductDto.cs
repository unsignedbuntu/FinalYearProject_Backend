using System;

namespace KTUN_Final_Year_Project.DTOs
{
    public class ReviewableProductDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        // Frontend'de gerekebilecek diğer ürün veya sipariş detayları eklenebilir.
        // Örneğin, ürünün varyant bilgileri (renk, boyut vb.) varsa onlar da eklenebilir.
        public string? ProductVariantInfo { get; set; } // Örnek
    }
} 