namespace KTUN_Final_Year_Project.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class OrderItemCreationDTO
    {
#nullable disable
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal PriceAtPurchase { get; set; }
#nullable enable
    }
} 