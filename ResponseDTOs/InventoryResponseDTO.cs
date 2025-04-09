namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class InventoryResponseDTO
    {
        public int InventoryID { get; set; }
        
        public int ProductID { get; set; }
        
        public string ChangeType { get; set; } = string.Empty;
        
        public int QuantityChanged { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public DateTime ChangeDate { get; set; }
        
        public bool Status { get; set; }
        
        // Ürün bilgileri
        public string ProductName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string? StoreName { get; set; }
        public int CurrentStock { get; set; }
    }
}
