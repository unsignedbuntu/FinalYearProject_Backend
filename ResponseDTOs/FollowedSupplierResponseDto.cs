namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class FollowedSupplierResponseDto
    {
        public int SupplierID { get; set; }
        public string? SupplierName { get; set; } // Nullable yapalım, tedarikçi adı her zaman dolu olmayabilir (teorik olarak)
        public string? ContactEmail { get; set; }
        // İhtiyaç duyulursa tedarikçinin diğer bilgileri (logo URL'si vb.) buraya eklenebilir.
        // public string? LogoUrl { get; set; }
        public DateTime FollowedDate { get; set; } // Takip edilme tarihi de eklenebilir
    }
} 