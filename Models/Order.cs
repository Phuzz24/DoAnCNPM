    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
            public string CustomerName { get; set; } // Tên người dùng (người đặt hàng)
            public string CustomerPhone { get; set; }       // Số điện thoại người nhận



            [Required]
            [StringLength(50)]
            [Column("status")]

            public string Status { get; set; } // Trạng thái đơn hàng (Pending, Completed, Canceled, etc.)

            [Required]
            [DataType(DataType.Currency)]
            [Column("total_amount")]

            public decimal? TotalAmount { get; set; } // Tổng số tiền đơn hàng

            // Thông tin giao hàng (Shipment)
            [StringLength(50)]
            [Column("shipment_status")]

            public string ShipmentStatus { get; set; } // Trạng thái giao hàng (In Transit, Delivered, etc.)
            [Column("delivery_date")]


            public DateTime? DeliveryDate { get; set; } // Ngày giao hàng (nullable)
            [Column("tracking_number")]


            [StringLength(100)]
            public string TrackingNumber { get; set; } // Mã vận chuyển

            // Thêm địa chỉ giao hàng và phí giao hàng
            public string ShippingAddress { get; set; } // Địa chỉ giao hàng
            public decimal? ShippingFee { get; set; } // Phí giao hàng

            // Thông tin thanh toán (Payment)
            [StringLength(50)]
            [Column("payment_method")]

            public string PaymentMethod { get; set; } // Phương thức thanh toán (COD, Online Payment, etc.)

            [StringLength(50)]
            [Column("payment_status")]

            public string PaymentStatus { get; set; } // Trạng thái thanh toán (Paid, Unpaid, etc.)
            [Column("payment_date")]


            public DateTime? PaymentDate { get; set; } // Ngày thanh toán (nullable)

            // Navigation properties
            public Customer Customer { get; set; } // Liên kết tới bảng Customer

            public ICollection<OrderDetail> OrderDetails { get; set; } // Liên kết tới các sản phẩm trong đơn hàng
        }
    }
