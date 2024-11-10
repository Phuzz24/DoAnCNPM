using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace DoAnCNPM.Models
{

public class Brand
{
    [Key]
    [Column("brand_id")]

    public int Brand_ID { get; set; } // Khóa chính
    [Column("brand_name")]
        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        public string BrandName { get; set; }

        // Navigation properties
        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public ICollection<Product> Products { get; set; }
}
}
