using DoAnCNPM.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Customer
{
    [Key]
    public int Customer_ID { get; set; } // Khóa chính
    public int User_ID { get; set; }
    public string NameCus { get; set; }
    public string AddressCus { get; set; }
    public string PhoneCus { get; set; }
    public string EmailCus { get; set; }

    // Navigation properties
    public User User { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
    public ICollection<Cart> Carts { get; set; }
}
