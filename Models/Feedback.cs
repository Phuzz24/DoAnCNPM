using System.ComponentModel.DataAnnotations;

public class Feedback
{
    [Key]
    public int Feedback_ID { get; set; } // Khóa chính
    public int Customer_ID { get; set; }
    public int Product_ID { get; set; }
    public string Comment { get; set; }
    public DateTime FeedbackDate { get; set; }

    // Navigation properties
    public Customer Customer { get; set; }
    public Product Product { get; set; }
}
