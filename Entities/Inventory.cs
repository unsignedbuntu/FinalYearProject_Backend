namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Inventory
    {
#nullable disable 

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int InventoryID { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        
        public virtual Products Product {  get; set; }

        public enum ChangeTypeEnum
        {
            IN,
            OUT
        }
        
        public ChangeTypeEnum ChangeType { get; set; }

        public int QuantityChanged { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public bool Status { get; set; } = true;
        public Inventory() { }
    }
}
