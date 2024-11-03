namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductRecommendations
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int RecommendationID {  get; set; }

        public int UserID {  get; set; }

        public int ProductID { get; set; }

        [ForeignKey("UserID")]
        
        public virtual Users User { get; set; }


        [ForeignKey("ProductID")]

         public virtual Products Product { get; set; }

        public DateTime RecommendationDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;

        public ProductRecommendations() { }

    }
}
