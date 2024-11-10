using System;
using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;


namespace DoAnCNPM.Models
    {
        public class Order
        {
            [Key]
            [Column("order_id")]

            public int Order_ID { get; set; } // Khóa chính
        [ForeignKey("Customer")]

        [Column("customer_id")]


            // Khóa ngoại tới Customer
            public int Customer_ID { get; set; }
            [Column("order_date")]


            [Required]
            public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]

        public string CustomerName { get; set; } // Tên người dùng (người đặt hàng)
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]

        public string CustomerPhone { get; set; }       // Số điện thoại người nhận



            [Required]
            [StringLength(50)]
            [Column("status")]

        public string Status { get; set; } = "Pending"; // Trạng thái mặc định

        [Required]
            [DataType(DataType.Currency)]
            [Column("total_amount")]

        public decimal TotalAmount { get; set; } = 0; // Tổng tiền mặc định là 0

        // Thông tin giao hàng (Shipment)
        [StringLength(50)]
            [Column("shipment_status")]

        public string ShipmentStatus { get; set; } = "Chưa giao"; // Trạng thái giao hàng mặc định
        [Column("delivery_date")]


            public DateTime? DeliveryDate { get; set; } // Ngày giao hàng (nullable)
            [Column("tracking_number")]


            [StringLength(100)]
            public string TrackingNumber { get; set; } // Mã vận chuyển

        // Thêm địa chỉ giao hàng và phí giao hàng
        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]

        public string ShippingAddress { get; set; } // Địa chỉ giao hàng
        [Required]

        public decimal ShippingFee { get; set; } = 30000; // Phí giao hàng mặc định

        // Thông tin thanh toán (Payment)
        [Required]

        [StringLength(50)]
            [Column("payment_method")]

            public string PaymentMethod { get; set; } // Phương thức thanh toán (COD, Online Payment, etc.)

            [StringLength(50)]
            [Column("payment_status")]

            public string PaymentStatus { get; set; } // Trạng thái thanh toán (Paid, Unpaid, etc.)
        [NotMapped]    
        public string ShippingMethod
        {
            get => ShippingFee == 50000 ? "express" : "standard";
            set => ShippingFee = value == "express" ? 50000 : 30000;
        }
        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; } // Ngày thanh toán (nullable)

        // Navigation properties
        [JsonIgnore]
        [ValidateNever]
        public Customer Customer { get; set; } // Liên kết tới bảng Customer
        [JsonIgnore]
        [ValidateNever]


        public ICollection<OrderDetail> OrderDetails { get; set; } // Liên kết tới các sản phẩm trong đơn hàng
        }
    }
