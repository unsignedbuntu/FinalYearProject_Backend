namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class InventoryDTO
    {
#nullable disable
        [Required]
        public int ProductID { get; set; }

        [MaxLength(50)]
        public string ChangeType { get; set; }

        [Required]
        public int QuantityChanged { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
