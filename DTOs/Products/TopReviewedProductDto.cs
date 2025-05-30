namespace KTUN_Final_Year_Project.DTOs.Products // Namespace'i projenize göre ayarlayın
{
    public class TopReviewedProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; } // İsteğe bağlı: Frontend'de indirim hesaplamak için
        public string ImageUrl { get; set; } = string.Empty; // Frontend tarafından erişilebilir bir URL veya yol olduğundan emin olun
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; } // Hata ayıklama veya görüntüleme için faydalı olabilir
    }
} 