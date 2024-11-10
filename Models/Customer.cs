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
        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng.")]

        public string NameCus { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]

        public string AddressCus { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]

        public string PhoneCus { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
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