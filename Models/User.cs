using System.ComponentModel.DataAnnotations;

namespace DoAnCNPM.Models
{
    public class User
    {
        [Key]
        public int User_ID { get; set; }  // Khóa chính
        public string Username { get; set; }  // Tên đăng nhập
        public string Password { get; set; }  // Mật khẩu (mã hóa)
        public string Role { get; set; }  // Vai trò của người dùng (Admin, Khách hàng, Nhân viên)

        // Các thuộc tính điều hướng nếu cần
        public ICollection<Admin> Admins { get; set; }
        public ICollection<Staff> Staffs { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
