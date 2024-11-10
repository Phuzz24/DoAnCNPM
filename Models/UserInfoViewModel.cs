namespace DoAnCNPM.Models
{
    public class UserInfoViewModel
    {
        public string Username { get; set; }  // Tên đăng nhập, không cho chỉnh sửa
        public string NameCus { get; set; }   // Tên khách hàng
        public string AddressCus { get; set; } // Địa chỉ khách hàng
        public string PhoneCus { get; set; }   // Số điện thoại khách hàng
        public string EmailCus { get; set; }   // Email khách hàng
    }

}