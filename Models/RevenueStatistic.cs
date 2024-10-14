using System.ComponentModel.DataAnnotations;

public class RevenueStatistic
{
    [Key]
    public int Revenue_ID { get; set; } // Khóa chính
    public DateTime ReportDate { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProductsSold { get; set; }
}
