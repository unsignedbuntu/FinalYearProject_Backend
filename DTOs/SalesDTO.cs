namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class SalesDTO
    {
#nullable disable
        [Required]
        public int UserID { get; set; }

        [Required]
        public int StoreID { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar 0 veya daha büyük olmalıdır.")]
        public decimal TotalAmount { get; set; }
        
        // Satış detayları koleksiyonu
        public List<SalesDetailsDTO> SalesDetails { get; set; }
    }
}
