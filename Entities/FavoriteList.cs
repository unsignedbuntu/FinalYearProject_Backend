using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // ICollection için eklendi

namespace KTUN_Final_Year_Project.Entities
{
    public class FavoriteList
    {
        [Key]
        public int FavoriteListID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string ListName { get; set; } = null!;

        [Required]
        public bool IsPrivate { get; set; } = false;

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool Status { get; set; } = true;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }

        public virtual ICollection<FavoriteListItem>? FavoriteListItems { get; set; }
    }
} 