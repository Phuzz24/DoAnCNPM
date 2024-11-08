using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCNPM.Models
{
    public class Feedback
    {
        [Key]
        [Column("Feedback_ID")]

        public int Feedback_ID { get; set; } // Khóa chính
        [ForeignKey("Customer")]
        [Column("Customer_ID")]

        public int Customer_ID { get; set; }
        [ForeignKey("Product")]
        [Column("Product_ID")]

        public int Product_ID { get; set; }
        [Column("comment")]
        [MaxLength(500)] // Đảm bảo độ dài ký tự phù hợp với cơ sở dữ liệu

        public string Comment { get; set; }
        [Column("feedback_date")]

        public DateTime FeedbackDate { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
