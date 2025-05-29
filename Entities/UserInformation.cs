using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    [Table("UserInformation")]
    public class UserInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserInformationID { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign key to Users table

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)] // Telefon numarası için uzunluk
        public string? PhoneNumber { get; set; } // Yeni alan

        // Navigation property
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
    }
} 