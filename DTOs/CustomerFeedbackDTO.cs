namespace KTUN_Final_Year_Project.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomerFeedbackDTO
    {
#nullable disable
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int CustomerFeedbackID { get; set; }

        public string FeedbackText { get; set; }

        public int Rating { get; set; }

        public DateTime FeedbackDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        public bool Status { get; set; } = true;

        public virtual UsersDTO Users { get; set; }

        public virtual ProductsDTO Products { get; set; }
    }
}
