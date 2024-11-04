namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class SuppliersResponseDTO
    {
#nullable disable
        public String SupplierName { get; set; }

        public String ContactEmail { get; set; }
    }
}
