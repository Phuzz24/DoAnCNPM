using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Authorization;


namespace DoAnCNPM.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public OrderManagementController(DBDienThoaiContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Nhân viên,Admin")]
        public async Task<IActionResult> DSDonHang(string searchTerm = "", string orderStatus = "", string paymentStatus = "", int page = 1, int pageSize = 10)
        {
            var orders = _context.Orders.AsQueryable();

            // Lọc đơn hàng theo từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => o.CustomerName.Contains(searchTerm) || o.ShippingAddress.Contains(searchTerm));
            }

            // Lọc đơn hàng theo trạng thái
            if (!string.IsNullOrEmpty(orderStatus))
            {
                orders = orders.Where(o => o.Status == orderStatus);
            }

            // Lọc đơn hàng theo tình trạng thanh toán
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orders = orders.Where(o => o.PaymentStatus == paymentStatus);
            }

            // Tổng số đơn hàng
            int totalItems = orders.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Phân trang
            var paginatedOrders = await orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(paginatedOrders);
        }

        [Authorize(Roles = "Admin")]


        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách khách hàng từ cơ sở dữ liệu và gán vào ViewBag.Customers
            ViewBag.Customers = _context.Customers.ToList();

            // Lấy danh sách sản phẩm từ cơ sở dữ liệu và gán vào ViewBag.Products nếu cần
            ViewBag.Products = _context.Products.ToList();

            return View();
        }

        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Customer_ID, string CustomerName, string CustomerPhone, string ShippingAddress, string ShippingMethod, string PaymentMethod, Dictionary<int, int> ProductQuantities)
        {
            var errorMessages = new List<string>();

            if (Customer_ID == 0)
                errorMessages.Add("Vui lòng chọn khách hàng.");

            if (string.IsNullOrWhiteSpace(CustomerName))
                errorMessages.Add("Tên người nhận là bắt buộc.");

            if (string.IsNullOrWhiteSpace(CustomerPhone))
                errorMessages.Add("Số điện thoại là bắt buộc.");

            if (string.IsNullOrWhiteSpace(ShippingAddress))
                errorMessages.Add("Địa chỉ giao hàng là bắt buộc.");

            if (string.IsNullOrWhiteSpace(ShippingMethod))
                errorMessages.Add("Phương thức giao hàng là bắt buộc.");

            if (string.IsNullOrWhiteSpace(PaymentMethod))
                errorMessages.Add("Phương thức thanh toán là bắt buộc.");

            if (ProductQuantities == null || ProductQuantities.Count == 0 || ProductQuantities.All(pq => pq.Value <= 0))
                errorMessages.Add("Vui lòng chọn ít nhất một sản phẩm và số lượng lớn hơn 0.");

            if (errorMessages.Any())
            {
                TempData["ErrorMessages"] = string.Join("<br/>", errorMessages);
                return RedirectToAction("Create");
            }

            // Tạo chuỗi ngẫu nhiên cho tracking_number
            var trackingNumber = GenerateRandomTrackingNumber();

            var order = new Order
            {
                Customer_ID = Customer_ID,
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                ShippingAddress = ShippingAddress,
                OrderDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(5),
                Status = "Pending",
                ShipmentStatus = "Chưa giao",
                PaymentMethod = PaymentMethod,
                PaymentStatus = PaymentMethod == "Online" ? "Đã thanh toán" : "Chưa thanh toán",
                ShippingFee = ShippingMethod == "express" ? 50000 : 30000,
                TotalAmount = 0,
                TrackingNumber = trackingNumber,
                PaymentDate = PaymentMethod == "Online" ? DateTime.Now : (DateTime?)null
            };

            // Lưu đơn hàng vào cơ sở dữ liệu để có Order_ID
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal calculatedTotalAmount = 0m;

            foreach (var entry in ProductQuantities)
            {
                var product = await _context.Products.FindAsync(entry.Key);
                if (product == null) continue;

                decimal price = product.Price; // Sử dụng trực tiếp product.Price mà không cần null-check
                int quantity = entry.Value;

                var orderDetail = new OrderDetail
                {
                    Order_ID = order.Order_ID,
                    Product_ID = product.Product_ID,
                    Quantity = quantity,
                    Price = price,
                    NamePro = product.NamePro ?? "Không có tên sản phẩm"
                };

                _context.OrderDetails.Add(orderDetail);
                calculatedTotalAmount += price * quantity;
            }

            // Cập nhật tổng tiền của đơn hàng
            order.TotalAmount = calculatedTotalAmount + order.ShippingFee;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
            return RedirectToAction(nameof(DSDonHang));
        }


        // Hàm tạo tracking_number ngẫu nhiên
        private string GenerateRandomTrackingNumber()
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    return new string(Enumerable.Repeat(chars, 8)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}

        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Order_ID == id);

            if (order == null)
            {
                return RedirectToAction("Error", new { message = "Không tìm thấy ID đơn hàng." });
            }

            // Lấy danh sách khách hàng và sản phẩm để hiển thị trong dropdown
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.Products = await _context.Products.ToListAsync();
            foreach (var detail in order.OrderDetails)
            {
                detail.Price ??= 0; // Giá trị mặc định
                detail.NamePro ??= "Không có tên sản phẩm";
                if (detail.Product == null)
                {
                    detail.Product = new Product
                    {
                        NamePro = "Không có thông tin sản phẩm"
                    };
                }
            }
            return View(order);
        }

        [Authorize(Roles = "Admin")]


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order, Dictionary<int, int> ProductQuantities)
        {
            if (id != order.Order_ID)
            {
                TempData["ErrorMessage"] = "Không tìm thấy ID đơn hàng.";
                return RedirectToAction("Error", new { message = "Không tìm thấy ID đơn hàng." });
            }

            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Order_ID == id);

            if (existingOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng để cập nhật.";
                return RedirectToAction("Error");
            }

            if (existingOrder.Status == "Đã hủy")
            {
                TempData["ErrorMessage"] = "Đơn hàng đã hủy không thể cập nhật.";
                return RedirectToAction("DSDonHang");
            }

            existingOrder.PaymentStatus = order.PaymentStatus;
            existingOrder.ShipmentStatus = order.ShipmentStatus;

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            decimal totalAmount = 0m;
            foreach (var entry in ProductQuantities)
            {
                var product = await _context.Products.FindAsync(entry.Key);
                if (product == null) continue;

                var orderDetail = new OrderDetail
                {
                    Order_ID = existingOrder.Order_ID,
                    Product_ID = product.Product_ID,
                    Quantity = entry.Value,
                    Price = product.Price,
                    NamePro = product.NamePro
                };

                _context.OrderDetails.Add(orderDetail);
                totalAmount += product.Price * entry.Value;
            }

            existingOrder.TotalAmount = totalAmount + existingOrder.ShippingFee;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
            return RedirectToAction(nameof(DSDonHang));
        }




        // Phương thức Error để hiển thị trang lỗi
        public IActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View("Error");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Tìm đơn hàng cần xóa
                var order = await _context.Orders
                    .Include(o => o.OrderDetails) // Bao gồm các chi tiết đơn hàng liên quan
                    .FirstOrDefaultAsync(o => o.Order_ID == id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại." });
                }

                // Xóa các chi tiết đơn hàng liên quan trước
                _context.OrderDetails.RemoveRange(order.OrderDetails);

                // Xóa đơn hàng
                _context.Orders.Remove(order);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đơn hàng và các chi tiết liên quan đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Order_ID == id);

            if (order == null)
            {
                return NotFound();
            }

            return PartialView("_OrderDetails", order);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            if (order.Status == "Đã hủy")
            {
                return Json(new { success = false, message = "Đơn hàng đã hủy không thể thay đổi trạng thái." });
            }

            order.ShipmentStatus = status;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái thành công." });
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Json(new { success = false, message = "Không có đơn hàng nào được chọn để xóa." });
                }

                var ordersToDelete = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Where(o => ids.Contains(o.Order_ID))
                    .ToListAsync();

                if (ordersToDelete.Count > 0)
                {
                    foreach (var order in ordersToDelete)
                    {
                        _context.OrderDetails.RemoveRange(order.OrderDetails);
                    }

                    _context.Orders.RemoveRange(ordersToDelete);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Đã xóa các đơn hàng được chọn thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng nào để xóa." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

    }


}

