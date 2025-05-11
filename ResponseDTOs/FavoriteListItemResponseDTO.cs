using System;

namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class FavoriteListItemResponseDTO
    {
        public int FavoriteListItemID { get; set; }
        public int FavoriteListID { get; set; }
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductImageUrl { get; set; }
        public bool InStock { get; set; }
        public DateTime AddedDate { get; set; }
    }
} 