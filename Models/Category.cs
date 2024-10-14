using System.ComponentModel.DataAnnotations;

namespace DoAnCNPM.Models
{
    public class Category
    {
        [Key]
        public int Category_ID { get; set; }
        public string CategoryName { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; }  // Một loại sản phẩm có thể có nhiều sản phẩm
    }

}
