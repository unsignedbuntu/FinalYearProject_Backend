namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ImageCacheResponseDTO
    {
#nullable disable
        public string PageID { get; set; }
        public string Prompt { get; set; }
        public byte[] Image { get; set; }

        [StringLength(64)]
        public string HashValue { get; set; }
    }
}
