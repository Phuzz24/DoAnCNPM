using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;


namespace DoAnCNPM.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public OrderManagementController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // GET: Order/Index
        public async Task<IActionResult> DSDonHang(string searchTerm)
        {
            var orders = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => o.CustomerName.Contains(searchTerm) ||
                                           o.Status.Contains(searchTerm) ||
                                           o.PaymentMethod.Contains(searchTerm));
            }

            return View(await orders.Include(o => o.Customer).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách khách hàng từ cơ sở dữ liệu và gán vào ViewBag.Customers
            ViewBag.Customers = _context.Customers.ToList();

            // Lấy danh sách sản phẩm từ cơ sở dữ liệu và gán vào ViewBag.Products nếu cần
            ViewBag.Products = _context.Products.ToList();

            return View();
        }


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

            return View(order);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order, Dictionary<int, int> ProductQuantities)
        {
            if (id != order.Order_ID)
            {
                TempData["ErrorMessage"] = "Không tìm thấy ID đơn hàng.";
                return RedirectToAction("Error", new { message = "Không tìm thấy ID đơn hàng." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                ViewBag.ErrorMessages = errors;

                ViewBag.Customers = await _context.Customers.ToListAsync();
                ViewBag.Products = await _context.Products.ToListAsync();
                order.OrderDetails = await _context.OrderDetails
                    .Where(od => od.Order_ID == id)
                    .Include(od => od.Product)
                    .ToListAsync();

                return View(order);
            }

            try
            {
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Order_ID == id);

                if (existingOrder == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng để cập nhật.";
                    return RedirectToAction("Error");
                }

                existingOrder.Customer_ID = order.Customer_ID;
                existingOrder.CustomerName = order.CustomerName;
                existingOrder.CustomerPhone = order.CustomerPhone;
                existingOrder.ShippingAddress = order.ShippingAddress;
                existingOrder.PaymentMethod = order.PaymentMethod;
                existingOrder.PaymentStatus = order.PaymentStatus;
                existingOrder.TrackingNumber = order.TrackingNumber;
                existingOrder.ShippingFee = order.ShippingFee;
                existingOrder.ShipmentStatus = order.ShipmentStatus;

                _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
                decimal totalAmount = 0m;

                foreach (var entry in ProductQuantities)
                {
                    var product = await _context.Products.FindAsync(entry.Key);
                    if (product == null) continue;

                    decimal price = product.Price;
                    int quantity = entry.Value;

                    var orderDetail = new OrderDetail
                    {
                        Order_ID = existingOrder.Order_ID,
                        Product_ID = product.Product_ID,
                        Quantity = quantity,
                        Price = price,
                        NamePro = product.NamePro
                    };

                    _context.OrderDetails.Add(orderDetail);
                    totalAmount += price * quantity;
                }

                existingOrder.TotalAmount = totalAmount + existingOrder.ShippingFee;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction(nameof(DSDonHang));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật đơn hàng: " + ex.Message;
                return RedirectToAction("Error");
            }
        }




        // Phương thức Error để hiển thị trang lỗi
        public IActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View("Error");
        }


        // GET: Order/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Order_ID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Order_ID == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.OrderDetails.RemoveRange(order.OrderDetails); // Xóa các chi tiết liên quan
            _context.Orders.Remove(order); // Xóa đơn hàng
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa đơn hàng thành công!";
            return RedirectToAction(nameof(DSDonHang));
        }
    }
}
