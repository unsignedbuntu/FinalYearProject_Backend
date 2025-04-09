namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class SuppliersResponseDTO
    {
        public int SupplierID { get; set; }
        
        public string SupplierName { get; set; } = string.Empty;
        
        public string? ContactEmail { get; set; }
        
        public bool Status { get; set; }

        // İlişkili Ürün Sayısı
        public int ProductCount { get; set; }
    }
}
