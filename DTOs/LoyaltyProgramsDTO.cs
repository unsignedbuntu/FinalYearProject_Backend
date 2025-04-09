namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LoyaltyProgramsDTO
    {
#nullable disable
        [Required]
        [MaxLength(100)]
        public string ProgramName { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100, ErrorMessage = "İndirim oranı 0-100 arasında olmalıdır.")]
        public decimal DiscountRate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Puan çarpanı en az 1 olmalıdır.")]
        public int PointsMultiplier { get; set; }
    }
}
