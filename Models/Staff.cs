using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


namespace DoAnCNPM.Models
{

public class Staff
{
    [Key]
    public int Staff_ID { get; set; } // Khóa chính
    [ForeignKey("User")]
    [Column("User_ID")]
    public int User_ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên nhân viên.")]

        public string NameStaff { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]

        public string AddressStaff { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]

        public string PhoneStaff { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string EmailStaff { get; set; }

        // Navigation properties
        [JsonIgnore] // Ngăn lỗi khi tạo Staff do thiếu User
        [ValidateNever]
        public User User { get; set; }
}
}
