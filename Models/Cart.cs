using System.ComponentModel.DataAnnotations;

public class Cart
{
    [Key]
    public int Cart_ID { get; set; } // Khóa chính
    public int Customer_ID { get; set; }
    public int Product_ID { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; }

    // Navigation properties
    public Customer Customer { get; set; }
    public Product Product { get; set; }
}
