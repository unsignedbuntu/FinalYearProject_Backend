using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class SupportMessagesDTO
    {
        [Required]
        public string UserID { get; set; } = default!;

        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Konu 3-255 karakter arasında olmalıdır.")]
        public string Subject { get; set; } = default!;

        [Required]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Mesaj 10-4000 karakter arasında olmalıdır.")]
        public string Message { get; set; } = default!;
    }

    public class SupportMessageReplyDTO
    {
        [Required]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Yanıt 10-4000 karakter arasında olmalıdır.")]
        public string Response { get; set; } = default!;
    }
} 