namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ReviewsResponseDTO
    {
        public int ReviewID { get; set; }
        
        public int UserID { get; set; }
        
        public int ProductID { get; set; }
        
        public int? OrderItemID { get; set; }
        
        public int Rating { get; set; }
        
        public string? Comment { get; set; }
        
        public DateTime ReviewDate { get; set; }
        
        // Kullanıcı ve ürün bilgileri
        public string? UserFullName { get; set; }
        public string? UserAvatarUrl { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
    }
} 