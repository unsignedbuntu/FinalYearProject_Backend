namespace KTUN_Final_Year_Project.DTOs
{
    public class FavoriteDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool InStock { get; set; }
        public DateTime AddedDate { get; set; }
    }
} 