namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class CategoriesResponseDTO
    {
        public int CategoryID { get; set; }
        
        public int StoreID { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;
                
        public bool Status { get; set; }
        
        // Store bilgileri
        public string StoreName { get; set; } = string.Empty;
        
        // Ürün sayısı
        public int ProductCount { get; set; }
    }
}
