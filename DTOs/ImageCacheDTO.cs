namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ImageCacheDTO
    {
#nullable disable
        [MaxLength(100)]
        public string PageID { get; set; }

        [Required]
        public string Prompt { get; set; }

        [Required]
        public byte[] Image { get; set; }
    }
}
