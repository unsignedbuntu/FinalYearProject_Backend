namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SalesResponseDTO
    {
#nullable disable
        
        public int UserID { get; set; }

        public int StoreID { get; set; }


        [ForeignKey("UserID")]
        public virtual UsersDTO Users { get; set; }

        [ForeignKey("StoreID")]
        public virtual StoresDTO Stores { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public decimal TotalAmount { get; set; }

    }
}
