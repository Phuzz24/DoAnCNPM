using System.ComponentModel.DataAnnotations;

public class Payment
{
    [Key]
    public int Payment_ID { get; set; } // Khóa chính
    public int Order_ID { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }

    // Navigation properties
    public Order Order { get; set; }
}
