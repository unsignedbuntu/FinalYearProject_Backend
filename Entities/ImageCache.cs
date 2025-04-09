using System;
using System.ComponentModel.DataAnnotations;

namespace KTUN_Final_Year_Project.Entities
{
    public class ImageCache
    {
        [Key]
        public int ID { get; set; }
        
        [MaxLength(100)]
        public string? PageID { get; set; }
        
        public string? Prompt { get; set; }
        
        public byte[]? Image { get; set; }
        
        [MaxLength(64)]
        public string? HashValue { get; set; }
        
        public bool Status { get; set; } = true;
    }
}