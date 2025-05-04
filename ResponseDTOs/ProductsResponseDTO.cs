namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ProductsResponseDTO
    {
        public int ProductID { get; set; }
        
        public string ProductName { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        
        public int StockQuantity { get; set; }
        
        public string? Barcode { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool Status { get; set; }
        
        // İlişki bilgileri
        public string? StoreName { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? SupplierNames { get; set; }
    }
}
