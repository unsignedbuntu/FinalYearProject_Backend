namespace KTUN_Final_Year_Project.ResponseDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class UsersResponseDTO
    {
#nullable disable
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string NFC_CardID { get; set; }
    }
}
