using DoAnCNPM.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Order
{
    [Key]
    public int Order_ID { get; set; } // Khóa chính
    public int Customer_ID { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }

    // Navigation properties
    public Customer Customer { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; }
    public ICollection<Payment> Payments { get; set; }
    public ICollection<Shipment> Shipments { get; set; }
}
