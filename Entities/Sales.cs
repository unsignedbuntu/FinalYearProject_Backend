﻿namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Sales
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleID {  get; set; }

        public int UserID { get; set; }

        public int StoreID { get; set; }

        [ForeignKey("UserID")]

        public virtual Users User { get; set; }

        [ForeignKey("StoreID")]

        public virtual Stores Store {  get; set; }
        public DateTime SaleDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public decimal TotalAmount { get; set; }

        public bool Status { get; set; } = true;

        public Sales() { }
    }
}
