using DoAnCNPM.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Brand
{
    [Key]
    public int Brand_ID { get; set; } // Khóa chính
    public string BrandName { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; }
}
