using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KTUN_Final_Year_Project.Entities; // Users ve Suppliers için

namespace KTUN_Final_Year_Project.Entities // Namespace'i projenize göre ayarlayın
{
    [Table("UserFollowedSuppliers")]
    public class UserFollowedSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserFollowedSupplierID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int SupplierID { get; set; }

        public DateTime FollowedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users User { get; set; }

        [ForeignKey("SupplierID")]
        public virtual Suppliers Supplier { get; set; }
    }
} 