namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

        public class ImageCache
        {
#nullable disable

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

            public int ID { get; set; }
            public string PageID { get; set; }
            public string Prompt { get; set; }
            public byte[] Image { get; set; } // Binary formatta resim verisi
           
            [StringLength(64)]
            public string HashValue { get; set; } // Prompt + negative prompt hash'i

            public bool Status { get; set; } = true;

            public ImageCache() { }

    }
}