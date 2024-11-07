namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductSuppliersResponseDTO
    {
#nullable disable

        public decimal SupplyPrice { get; set; }

        public DateTime SupplyDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        [ForeignKey("ProductID")]
        public int ProductID { get; set; }

        [ForeignKey("SupplierID")]

        public int SupplierID { get; set; }
    }
}
