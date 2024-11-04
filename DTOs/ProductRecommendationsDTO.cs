namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductRecommendationsDTO
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int RecommendationID { get; set; }

        public DateTime RecommendationDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;

        public virtual UsersDTO Users { get; set; }

        public virtual ProductsDTO Products { get; set; }

    }
}
