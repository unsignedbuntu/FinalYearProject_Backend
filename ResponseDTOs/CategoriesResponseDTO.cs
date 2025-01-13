namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class CategoriesResponseDTO
    {
#nullable disable
        public string CategoryName { get; set; }

        [ForeignKey("StoreID")]

        public int StoreID { get; set; }
    }
}
