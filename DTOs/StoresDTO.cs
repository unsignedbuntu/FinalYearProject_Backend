namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class StoresDTO
    {
#nullable disable
        [Required]
        [MaxLength(100)]
        public string StoreName { get; set; }
    }
}
