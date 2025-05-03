using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.DTOs
{
    public class AddFavoriteRequestDto
    {
        [Required]
        public int ProductId { get; set; }
    }
} 