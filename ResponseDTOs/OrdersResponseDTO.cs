namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public class OrdersResponseDTO
    {
        public int OrderID { get; set; }
        
        public int UserID { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = string.Empty;
        
        public string? ShippingAddress { get; set; }
        
        // Kullanıcı bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
} 