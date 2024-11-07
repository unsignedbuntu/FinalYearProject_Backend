namespace KTUN_Final_Year_Project.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomerFeedback
    {
#nullable disable

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int CustomerFeedbackID { get; set; }

       public int UserID { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("UserID")]

        public virtual Users User { get; set; }

        [ForeignKey("ProductID")]

        public virtual Products Product {  get; set; }  

        public string FeedbackText { get; set; }

        public int Rating { get; set; }

        public DateTime FeedbackDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        public bool Status { get; set; } = true;


        public CustomerFeedback() { }
    }
}
