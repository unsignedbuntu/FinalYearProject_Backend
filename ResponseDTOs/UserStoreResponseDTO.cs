namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    
    public class UserStoreResponseDTO
    {
        public int UserStoreID { get; set; }
        
        public int UserID { get; set; }
        
        public int StoreID { get; set; }
        
        public DateTime EnrollmentDate { get; set; }
        
        public bool Status { get; set; }
        
        // Kullanıcı ve mağaza bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
    }
}
