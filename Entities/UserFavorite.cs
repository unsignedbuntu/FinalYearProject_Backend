using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class UserFavorite
    {
        [Key]
        public int UserFavoriteID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int ProductID { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual Users User { get; set; } = null!;

        // Assuming Product entity exists in Entities/Product.cs
        // If not, this needs adjustment or Product entity creation.
        [ForeignKey("ProductID")]
        public virtual Products Product { get; set; } = null!;
    }
} 