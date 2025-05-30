using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class ReviewsDTO
    {
        // UserID backend'de claim'lerden alınacak.
        // public string UserID { get; set; } = default!;

        [Required]
        public int ProductID { get; set; }

        public int? OrderItemID { get; set; } // Yorumu belirli bir sipariş kalemine bağlamak için

        [Required]
        [Range(1, 5, ErrorMessage = "Değerlendirme puanı 1-5 arasında olmalıdır.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Yorum en fazla 1000 karakter olmalıdır.")]
        public string? Comment { get; set; } // Yorum opsiyonel
    }
} 