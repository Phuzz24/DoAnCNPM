using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


namespace DoAnCNPM.Models
{
    public class Cart
    {
        [Key]
        [Column("cart_id")]

        public int Cart_ID { get; set; } // Khóa chính
        [ForeignKey("Customer")]
        [Column("customer_id")]

        public int Customer_ID { get; set; }
        [ForeignKey("Product")]
        [Column("product_id")]

        public int Product_ID { get; set; }
        [Column("quantity")]

        public int Quantity { get; set; }
        [Column("added_at")]
        public DateTime AddedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
