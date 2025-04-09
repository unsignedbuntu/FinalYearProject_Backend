namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserStoreDTO
    {
#nullable disable
        [Required]
        public int UserID { get; set; }

        [Required]
        public int StoreID { get; set; }
    }
}
