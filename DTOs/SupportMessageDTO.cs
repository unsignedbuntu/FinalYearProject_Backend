using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class SupportMessageDTO
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Konu 3-255 karakter arasında olmalıdır.")]
        public string Subject { get; set; } = default!;

        [Required]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Mesaj 10-4000 karakter arasında olmalıdır.")]
        public string MessageContent { get; set; } = default!;
    }
} 