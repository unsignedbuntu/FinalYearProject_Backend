namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Stores
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int StoreID { get; set; }

        public string StoreName { get; set; }

        public string StoreType { get; set; }

        public bool Status { get; set; } = true;

        public Stores() { }
    }
}
