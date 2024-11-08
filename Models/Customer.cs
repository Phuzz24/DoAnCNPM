using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
namespace DoAnCNPM.Models
{



    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int Customer_ID { get; set; } // Khóa chính
        [ForeignKey("User")]

        public int User_ID { get; set; }
        public string NameCus { get; set; }
        public string AddressCus { get; set; }
        public string PhoneCus { get; set; }
        public string EmailCus { get; set; }

        // Navigation properties
        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public User User { get; set; }
        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public ICollection<Order> Orders { get; set; }
        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public ICollection<Feedback> Feedbacks { get; set; }

        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public ICollection<Cart> Carts { get; set; }
    }
}