using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class FavoriteListItemDTO
    {
        [Required(ErrorMessage = "Ürün ID'si zorunludur.")]
        public int ProductId { get; set; }
    }
} 