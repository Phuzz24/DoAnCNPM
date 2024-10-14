using System.ComponentModel.DataAnnotations;

public class Shipment
{
    [Key]
    public int Shipment_ID { get; set; } // Khóa chính
    public int Order_ID { get; set; }
    public DateTime ShipmentDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string ShipmentStatus { get; set; }
    public string TrackingNumber { get; set; }

    // Navigation properties
    public Order Order { get; set; }
}
