namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.Collections.Generic;
    
    public class SalesResponseDTO
    {
        public int SaleID { get; set; }
        
        public int UserID { get; set; }
        
        public int StoreID { get; set; }
        
        public DateTime SaleDate { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public bool Status { get; set; }
        
        // Kullanıcı ve mağaza bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        
        // Satış detayları
        public List<SalesDetailsResponseDTO> SalesDetails { get; set; } = new List<SalesDetailsResponseDTO>();
    }
}
