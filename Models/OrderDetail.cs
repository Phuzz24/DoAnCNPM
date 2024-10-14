using System.ComponentModel.DataAnnotations;

public class OrderDetail
{
    [Key]
    public int OrderDetail_ID { get; set; } // Khóa chính
    public int Order_ID { get; set; }
    public int Product_ID { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    // Navigation properties
    public Order Order { get; set; }
    public Product Product { get; set; }
}
