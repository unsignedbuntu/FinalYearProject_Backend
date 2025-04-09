namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.Collections.Generic;

    public class StoresResponseDTO
    {
        public int StoreID { get; set; }
        
        public string StoreName { get; set; } = string.Empty;
        
        public bool Status { get; set; }

        // İlişki Bilgileri (Countlar için)
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
    }
}
