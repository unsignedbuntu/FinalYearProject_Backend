﻿namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class ProductRecommendationsResponseDTO
    {
        public int RecommendationID { get; set; }
        
        public int UserID { get; set; }
        
        public int ProductID { get; set; }
        
        public DateTime RecommendationDate { get; set; }
        
        public bool Status { get; set; }
        
        // Kullanıcı ve ürün bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Barcode { get; set; }
    }
}
