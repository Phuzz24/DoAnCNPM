using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace DoAnCNPM.Models
{
    public class Category
    {
        [Key]
        [Column("category_id")]

        public int Category_ID { get; set; }
        [Column("category_name")]
        [Required(ErrorMessage = "Tên loại là bắt buộc.")]

        public string CategoryName { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ValidateNever]
        public ICollection<Product> Products { get; set; }  // Một loại sản phẩm có thể có nhiều sản phẩm
    }

}
