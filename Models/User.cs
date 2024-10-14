using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DoAnCNPM.Models
{
    // Kế thừa từ IdentityUser để sử dụng hệ thống Identity
    public class User : IdentityUser
    {
        // Bạn có thể thêm các thuộc tính khác nếu cần, như Role chẳng hạn, nhưng lưu ý rằng Identity đã có sẵn các thuộc tính cơ bản như UserName, PasswordHash.
        public int User_ID { get; set; }  // Khóa chính kiểu int
        // Các thuộc tính khác như trước
        public string Role { get; set; }

        // Navigation properties nếu bạn cần quản lý liên kết với Admin, Staff, Customers
        public ICollection<Admin> Admins { get; set; }
        public ICollection<Staff> Staffs { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
