using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class FavoriteListItem
    {
        [Key]
        public int FavoriteListItemID { get; set; }

        [Required]
        public int FavoriteListID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("FavoriteListID")]
        public virtual FavoriteList? FavoriteList { get; set; }

        [ForeignKey("ProductID")]
        public virtual Products? Product { get; set; }
    }
} 