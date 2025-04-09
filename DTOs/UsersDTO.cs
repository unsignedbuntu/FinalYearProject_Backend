namespace KTUN_Final_Year_Project.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class UsersDTO
    {
        public int Id { get; set; } // DB şemasında PK 'Id'
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? NFC_CardID { get; set; }
    }
}
