using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnCNPM.Models
{
    public class OrderViewModel
    {
        // Thông tin đơn hàng
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public string PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; } // Tổng số tiền

        
    }
}
