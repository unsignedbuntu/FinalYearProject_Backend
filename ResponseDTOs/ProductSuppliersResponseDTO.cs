namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ProductSuppliersResponseDTO
    {
        public int ProductSupplierID { get; set; }
        
        public int ProductID { get; set; }
        
        public int SupplierID { get; set; }
        
        public DateTime SupplyDate { get; set; }
        
        public bool Status { get; set; }
        
        // Ürün ve tedarikçi bilgileri
        public string ProductName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
    }
}
