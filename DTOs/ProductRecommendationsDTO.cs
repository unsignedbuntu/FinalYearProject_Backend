namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ProductRecommendationsDTO
    {
#nullable disable
        [Required]
        public int UserID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Öneri gücü 0'dan büyük olmalıdır.")]
        public decimal RecommendationStrength { get; set; } = 1.0m;
    }
}
