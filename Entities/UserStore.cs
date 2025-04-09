namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
    public class UserStore
    {
        [Key]
        public int UserStoreID { get; set; } // Bu alan DB'de yok ama EF Core many-to-many için isteyebilir.

        public int UserID { get; set; }

        public int StoreID { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        public bool Status { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }

        [ForeignKey("StoreID")]
        public virtual Stores? Store { get; set; }

        public UserStore() { }
    }
#nullable disable
}
