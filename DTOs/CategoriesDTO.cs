namespace KTUN_Final_Year_Project.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class CategoriesDTO
    {
#nullable disable
        [Required]
        public int StoreID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

    }
}
