using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCNPM.Models
{
    public class Category
    {
        [Key]
        [Column("category_id")]

        public int Category_ID { get; set; }
        [Column("category_name")]

        public string CategoryName { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; }  // Một loại sản phẩm có thể có nhiều sản phẩm
    }

}
