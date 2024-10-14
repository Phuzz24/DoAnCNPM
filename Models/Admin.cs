using System.ComponentModel.DataAnnotations;

namespace DoAnCNPM.Models
{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }
        public int User_ID { get; set; }
        public string AdminName { get; set; }
        public string AddressAd { get; set; }
        public string PhoneAd { get; set; }
        public string EmailAd { get; set; }

        // Navigation properties
        public User User { get; set; }
    }

}
