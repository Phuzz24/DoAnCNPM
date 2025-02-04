using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnCNPM.Models
{
    public class AdminActivity
    {
        [Key]
        public int Id { get; set; }
        public string Activity { get; set; } // Mô tả hoạt động
        public string AdminName { get; set; } // Tên admin thực hiện
        public DateTime Timestamp { get; set; } // Thời gian thực hiện
    }

}
