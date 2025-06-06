namespace KTUN_Final_Year_Project.DTOs
{
    public class CartItemDto
    {
        public int UserCartItemId { get; set; } // Include the cart item ID itself
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime AddedDate { get; set; }
        public bool InStock { get; set; } // Added based on Product.StockQuantity > 0
        // Add Supplier info if needed
         public required string SupplierName { get; set; }
    }
} 