﻿namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SuppliersDTO
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int SupplierID { get; set; }

        public String SupplierName { get; set; }

        public String ContactEmail { get; set; }

        public bool Status { get; set; } = true;
    }
}
