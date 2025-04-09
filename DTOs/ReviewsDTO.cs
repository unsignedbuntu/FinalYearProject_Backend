using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class ReviewsDTO
    {
        [Required]
        public string UserID { get; set; } = default!;

        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Değerlendirme puanı 1-5 arasında olmalıdır.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Yorum 10-1000 karakter arasında olmalıdır.")]
        public string Comment { get; set; } = default!;
    }
} 