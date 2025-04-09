namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.Collections.Generic;
    
    public class UsersResponseDTO
    {
        public int Id { get; set; }
        
        public string Email { get; set; } = default!;
        
        public string? PhoneNumber { get; set; }
        
        public string FullName { get; set; } = default!;
        
        public string? NFC_CardID { get; set; }
        
        public bool Status { get; set; }
        
        // Role bilgileri
        public List<string> Roles { get; set; } = new List<string>();
        
        // İstatistikler (Bunlar DB şemasında yok, kaldırılabilir veya farklı hesaplanabilir)
        // public int OrderCount { get; set; }
        // public int LoyaltyProgramCount { get; set; }
    }
}
