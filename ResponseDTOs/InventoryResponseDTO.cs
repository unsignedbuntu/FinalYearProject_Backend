namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class InventoryResponseDTO
    {
#nullable disable

        /* public enum ChangeTypeEnum
         {
             OUT OF STOCK,
             IN STOCK,
             LIMITED STOCK
         }
        */
        public string ChangeType { get; set; }

        public int QuantityChanged { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        [ForeignKey("ProductID")]

        public int ProductID { get; set; }
    }
}
