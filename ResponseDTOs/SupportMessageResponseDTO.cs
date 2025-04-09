using KTUN_Final_Year_Project.Entities;

namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class SupportMessageResponseDTO
    {
        public int SupportMessageID { get; set; }
        public string UserID { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string MessageContent { get; set; } = default!;
        public string? ReplyMessage { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public bool Status { get; set; }
    }
} 