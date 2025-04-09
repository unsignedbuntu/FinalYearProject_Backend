namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public class OrderItemsResponseDTO
    {
        public int OrderItemID { get; set; }
        
        public int OrderID { get; set; }
        
        public int ProductID { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal PriceAtPurchase { get; set; }
        
        // Ürün bilgileri
        public string ProductName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
    }
} 