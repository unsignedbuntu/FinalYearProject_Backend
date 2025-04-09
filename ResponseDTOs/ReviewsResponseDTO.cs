namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ReviewsResponseDTO
    {
        public int ReviewID { get; set; }
        
        public int UserID { get; set; }
        
        public int ProductID { get; set; }
        
        public int Rating { get; set; }
        
        public string? Comment { get; set; }
        
        public DateTime ReviewDate { get; set; }
        
        public bool Status { get; set; }
        
        // Kullanıcı ve ürün bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
    }
} 