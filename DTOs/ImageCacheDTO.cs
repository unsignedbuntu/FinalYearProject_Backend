namespace KTUN_Final_Year_Project.DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Bu DTO, resim cache'i oluşturma ve güncelleme işlemleri için kullanılacak.
    public class ImageCacheDTO
    {
#nullable disable
        [Required]
        public string Prompt { get; set; }

        [Required]
        public string Base64Image { get; set; } // Resim verisi base64 string olarak alınacak
#nullable enable
        // ProductId ve SupplierId yerine EntityType ve EntityId kullanılacak
        public string? EntityType { get; set; } // Örn: "Product", "Supplier" veya null olabilir (genel cache için)
        public int? EntityId { get; set; }     // İlişkili ProductID veya SupplierID (EntityType doluysa anlamlı)
    }
}
