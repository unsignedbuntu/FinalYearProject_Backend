namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class SupportMessagesResponseDTO
    {
        public int MessageID { get; set; }
        
        public int UserID { get; set; }
        
        public string Subject { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; }
        
        public string Status { get; set; } = string.Empty;
                
        // Kullanıcı bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
} 