using DoAnCNPM.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public int Product_ID { get; set; }  // Khóa chính
    public string NamePro { get; set; }
    public int? Brand_ID { get; set; }  // Khóa ngoại tới Brands
    public int? Category_ID { get; set; }  // Khóa ngoại tới Categories
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }

    // Navigation properties
    public Brand Brand { get; set; }  // Liên kết với bảng Brands
    public Category Category { get; set; }   // Liên kết với Category

    public ICollection<OrderDetail> OrderDetails { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
    public ICollection<Cart> Carts { get; set; }
}
