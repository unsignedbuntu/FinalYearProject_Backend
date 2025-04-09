using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class SupportMessages
    {
        [Key]
        public int MessageID { get; set; }
        
        public int UserID { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        [MaxLength(50)]
        public string Status { get; set; } = "Open";
        
        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
    }
} 