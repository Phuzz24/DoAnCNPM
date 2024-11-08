using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


namespace DoAnCNPM.Models
{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }
        [ForeignKey("User")]
        [Column("User_ID")]
        public int User_ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên admin.")]

        public string AdminName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]

        public string AddressAd { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]

        public string PhoneAd { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string EmailAd { get; set; }

        // Navigation properties
        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public User User { get; set; }
    }

}
