namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using KTUN_Final_Year_Project.DTOs;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductSuppliersResponseDTO
    {
#nullable disable

        public int ProductID { get; set; }

        public int SupplierID { get; set; }

        [ForeignKey("ProductID")]
        public virtual ProductsDTO Products { get; set; }

        [ForeignKey("SupplierID")]

        public virtual SuppliersDTO Suppliers { get; set; }

        public decimal SupplyPrice { get; set; }

        public DateTime SupplyDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }
}
