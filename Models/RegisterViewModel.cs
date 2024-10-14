using System.ComponentModel.DataAnnotations;

namespace DoAnCNPM.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }


}
