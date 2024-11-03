namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Categories
    {
#nullable disable
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public bool Status { get; set; } = true;

        public Categories() { }
    }
}
