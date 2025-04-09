namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ImageCacheResponseDTO
    {
        public int ID { get; set; }
        
        public string? PageID { get; set; }
        
        public string Prompt { get; set; } = string.Empty;
        
        public string? HashValue { get; set; }
        
        public bool Status { get; set; }
        
        // Base64 formatında resim verisi
        public string? Base64Image { get; set; }
    }
}
