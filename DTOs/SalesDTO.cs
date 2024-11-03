namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SalesDTO
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleID { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public decimal TotalAmount { get; set; }

        public bool Status { get; set; } = true;

        public virtual UsersDTO Users { get; set; }

        public virtual StoresDTO Stores { get; set; }
    }
}
