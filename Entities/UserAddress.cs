using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    [Table("UserAddresses")]
    public class UserAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserAddressID { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign key to Users table

        [Required]
        [StringLength(100)]
        public string AddressTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(100)]
        public string? District { get; set; }

        [Required]
        [StringLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool Status { get; set; } = true;

        // Navigation property
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
    }
} 