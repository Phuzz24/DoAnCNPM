using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DoAnCNPM.Models
{
    public class Product
    {
        [Key]
        public int Product_ID { get; set; }  // Khóa chính

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string NamePro { get; set; }

        [ForeignKey("Brand")]
        [Column("brand_id")]
        [Required(ErrorMessage = "Chọn một thương hiệu.")]
        public int? Brand_ID { get; set; }

        [ForeignKey("Category")]
        [Column("category_id")]
        [Required(ErrorMessage = "Chọn một danh mục.")]
        public int? Category_ID { get; set; }

        [Column("price")]
        [Range(0, 99999999999999.99, ErrorMessage = "Giá không hợp lệ. Giá trị quá lớn.")]
        public decimal Price { get; set; } = 0;

        [Column("quantity")]
        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0.")]

        public int Quantity { get; set; }


        [Column("description")]
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Description { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [NotMapped]
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public Brand Brand { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public Category Category { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [JsonIgnore]
        [ValidateNever]
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        [JsonIgnore]
        [ValidateNever]
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
