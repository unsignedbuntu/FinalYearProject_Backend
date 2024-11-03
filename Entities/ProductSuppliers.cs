namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductSuppliers
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ProductSupplierID { get; set; }

        public int ProductID { get; set; }

        public int SupplierID { get; set; }

        [ForeignKey("ProductID")]

        public virtual Products Product {  get; set; }

        [ForeignKey("SupplierID")]

        public virtual Suppliers Supplier { get; set; }

        public decimal SupplyPrice { get; set; }

        public DateTime SupplyDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;

        public ProductSuppliers() { }
    }
}
