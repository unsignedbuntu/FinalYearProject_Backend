namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class SalesDetailsResponseDTO
    {
        public int SaleDetailID { get; set; }
        
        public int SaleID { get; set; }
        
        public int StoreID { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal PriceAtSale { get; set; }
        
        public bool Status { get; set; }
        
        // Store ve Ürün bilgileri
        public string StoreName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
    }
}
