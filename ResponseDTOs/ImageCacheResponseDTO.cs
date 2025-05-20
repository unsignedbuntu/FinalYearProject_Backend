namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ImageCacheResponseDTO
    {
        public int ID { get; set; }
        
        public string? Prompt { get; set; } = string.Empty;
        
        public string? HashValue { get; set; }
        
        public bool Status { get; set; }
        
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        
        // Base64 formatında resim verisi
        public string? Base64Image { get; set; }

        // Resme doğrudan erişim için URL
        public string? ImageUrl { get; set; }
    }
}
