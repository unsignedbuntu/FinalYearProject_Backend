using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTUN_Final_Year_Project.Entities
{
    public class Users : IdentityUser<int>
    {
#nullable disable

        // UserID is inherited as 'Id' from IdentityUser<int>
        // Email is inherited from IdentityUser
        // PhoneNumber is inherited from IdentityUser

        // Keep custom properties
        public string FullName { get; set; }
        public string Address { get; set; }
        public string NFC_CardID { get; set; }
        public bool Status { get; set; } = true; // Renamed from IsActive for clarity potentially? Or keep Status? Let's keep Status for now.

        // Existing unique index configuration in DbContext needs to be updated if NFC_CardID should remain unique.

        // Constructor might not be needed anymore unless for specific initialization logic.
        // public Users() { }
    }
}
