using DoAnCNPM.Models;
using System.ComponentModel.DataAnnotations;

public class Staff
{
    [Key]
    public int Staff_ID { get; set; } // Khóa chính
    public int User_ID { get; set; }
    public string NameStaff { get; set; }
    public string AddressStaff { get; set; }
    public string PhoneStaff { get; set; }
    public string EmailStaff { get; set; }

    // Navigation properties
    public User User { get; set; }
}
