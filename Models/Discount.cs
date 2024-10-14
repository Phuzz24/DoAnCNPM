using System.ComponentModel.DataAnnotations;

public class Discount
{
    [Key]
    public int Discount_ID { get; set; } // Khóa chính
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
