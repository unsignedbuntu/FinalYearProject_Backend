using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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

        // Navigation Properties
        public virtual ICollection<Sales> Sales { get; set; } = new List<Sales>();
        public virtual ICollection<UserLoyalty> UserLoyalty { get; set; } = new List<UserLoyalty>();
        public virtual ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
        public virtual ICollection<ProductRecommendations> ProductRecommendations { get; set; } = new List<ProductRecommendations>();
        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public virtual ICollection<SupportMessages> SupportMessages { get; set; } = new List<SupportMessages>();

        // New navigation properties for UserInformation (one-to-one) and UserAddresses (one-to-many)
        public virtual UserInformation UserInformation { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

        // Existing unique index configuration in DbContext needs to be updated if NFC_CardID should remain unique.

        // Constructor might not be needed anymore unless for specific initialization logic.
        // public Users() { }
    }
}
