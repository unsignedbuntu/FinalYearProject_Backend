namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SuppliersDTO
    {
#nullable disable
        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; }
        
        [MaxLength(100)]
        [EmailAddress]
        public string ContactEmail { get; set; }
    }
}
