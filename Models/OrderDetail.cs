using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCNPM.Models
{

public class OrderDetail
{
    [Key]
        [Column("order_detail_id")]

        public int OrderDetail_ID { get; set; } // Khóa chính
    [ForeignKey("Order")]
        [Column("order_id")]


        public int Order_ID { get; set; }
    [ForeignKey("Product")]
        [Column("product_id")]

        public int Product_ID { get; set; }
        [Column("quantity")]

        public int Quantity { get; set; }
        [Column("price")]
        public decimal? Price { get; set; } = 0m;  // Sử dụng kiểu nullable decimal để tránh lỗi
        [Column("NamePro")]
        public string NamePro { get; set; } // Tên sản phẩm

        // Navigation properties
        public Order Order { get; set; }
    public Product Product { get; set; }
}
}

