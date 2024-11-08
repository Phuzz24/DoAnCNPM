using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCNPM.Models
{

public class Brand
{
    [Key]
    [Column("brand_id")]

    public int Brand_ID { get; set; } // Khóa chính
    [Column("brand_name")]

    public string BrandName { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; }
}
}
