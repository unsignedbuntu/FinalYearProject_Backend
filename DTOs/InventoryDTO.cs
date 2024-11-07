namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class InventoryDTO
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int InventoryID { get; set; }

      /*  public enum ChangeTypeEnum
        {
            IN,
            OUT
        }
      */
        public string ChangeType { get; set; }

        public int QuantityChanged { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;

        public virtual ProductsDTO Products { get; set; }
    }
}
