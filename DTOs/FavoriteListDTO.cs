using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class FavoriteListDTO
    {
        [Required(ErrorMessage = "Liste adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Liste adı en fazla 100 karakter olabilir.")]
        public string ListName { get; set; } = null!;

        public bool IsPrivate { get; set; } = false; // Default Public
    }
} 