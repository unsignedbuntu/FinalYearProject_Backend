using System;

namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class FavoriteListResponseDTO
    {
        public int FavoriteListID { get; set; }
        public int UserID { get; set; }
        public string? UserFullName { get; set; }
        public string ListName { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
} 