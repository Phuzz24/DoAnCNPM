using DoAnCNPM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;

namespace DoAnCNPM.Controllers
{
    public class Location
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    [Authorize]
    public class OrderController : Controller
    {
        private const string PayosUrl = "https://payos.vn/payment";
        private const string ClientId = "22118b21-b983-4ee3-a81b-f4f96ce7def5";
        private const string ApiKey = "fa32a6e4-ea66-4d96-a6dd-852b3ecd7cab";
        private const string ChecksumKey = "31c673922aa0f18cd33409fc08b37fd7a22f756c05c2d17128e2cbe4c6c1d607";
        private readonly DBDienThoaiContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(DBDienThoaiContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // Lấy danh sách tỉnh/thành phố
        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://provinces.open-api.vn/api/?depth=1");
            var provinces = JsonConvert.DeserializeObject<List<Location>>(response);
            return Json(provinces);
        }

        // Lấy danh sách quận/huyện theo mã tỉnh
        [HttpGet]
        public async Task<IActionResult> GetDistricts(string provinceCode)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"https://provinces.open-api.vn/api/p/{provinceCode}?depth=2");
            var province = JsonConvert.DeserializeObject<dynamic>(response);
            var districts = JsonConvert.DeserializeObject<List<Location>>(province["districts"].ToString());
            return Json(districts);
        }

        // Lấy danh sách xã/phường theo mã quận/huyện
        [HttpGet]
        public async Task<IActionResult> GetWards(string districtCode)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"https://provinces.open-api.vn/api/d/{districtCode}?depth=2");
            var district = JsonConvert.DeserializeObject<dynamic>(response);
            var wards = JsonConvert.DeserializeObject<List<Location>>(district["wards"].ToString());
            return Json(wards);
        }


    // Hiển thị trang thanh toán
    public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần phải đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy sản phẩm trong giỏ hàng
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.Customer_ID == customer.Customer_ID)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống.";
                return RedirectToAction("GioHang", "Cart");
            }

            ViewBag.Customer = customer;
            return View(cartItems);
        }

        public async Task<IActionResult> PlaceOrder(string CustomerName, string CustomerPhone, string Province, string District, string Ward, string DetailAddress, string ShippingMethod, string PaymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần phải đăng nhập để đặt hàng.";
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.Customer_ID == customer.Customer_ID)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống.";
                return RedirectToAction("GioHang", "Cart");
            }

            decimal totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            // Tính phí giao hàng dựa trên phương thức giao hàng
            decimal shippingFee = ShippingMethod == "express" ? 50000 : 30000;

            // Tạo địa chỉ giao hàng đầy đủ
            string fullAddress = $"{DetailAddress}, {Ward}, {District}, {Province}";

            // Tạo ngày đặt hàng và ngày giao hàng
            DateTime orderDate = DateTime.Now;
            DateTime deliveryDate = orderDate.AddDays(5);

            // Tạo mã vận chuyển ngẫu nhiên
            string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Tạo mới đơn hàng
            var order = new Order
            {
                Customer_ID = customer.Customer_ID,
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                ShippingAddress = fullAddress,
                OrderDate = orderDate,
                DeliveryDate = deliveryDate,
                TrackingNumber = trackingNumber,
                Status = "Pending",
                ShipmentStatus = "Chưa giao",
                PaymentStatus = PaymentMethod == "COD" ? "Chưa thanh toán" : "Đã thanh toán",
                PaymentMethod = PaymentMethod,
                ShippingFee = shippingFee,
                TotalAmount = totalAmount + shippingFee,
                PaymentDate = PaymentMethod == "Online" ? DateTime.Now : null // Đặt ngày thanh toán nếu là Online Payment
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Lưu chi tiết từng sản phẩm trong giỏ hàng vào OrderDetail
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Order_ID = order.Order_ID,
                    Product_ID = item.Product_ID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                _context.OrderDetails.Add(orderDetail);

                // Giảm số lượng sản phẩm trong bảng Product
                var product = await _context.Products.FindAsync(item.Product_ID);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    _context.Products.Update(product);
                }
            }

            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["OrderSuccess"] = true;
            return RedirectToAction("OrderConfirmation", new { id = order.Order_ID });
        }


        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            // Lấy đơn hàng theo ID
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Bao gồm chi tiết đơn hàng
                .ThenInclude(od => od.Product) // Bao gồm sản phẩm trong mỗi chi tiết
                .FirstOrDefaultAsync(o => o.Order_ID == id);

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            // Kiểm tra các giá trị có thể là NULL
            order.CustomerName ??= "Tên khách hàng không có";
            order.ShippingAddress ??= "Địa chỉ không có";
            order.PaymentMethod ??= "Không xác định";
            order.ShipmentStatus ??= "Không có thông tin";
            order.Status ??= "Không có thông tin";

            foreach (var detail in order.OrderDetails)
            {
                detail.Product.NamePro ??= "Tên sản phẩm không có";
                detail.Price ??= 0; // Gán giá trị mặc định nếu Price là NULL
            }

            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> FilterOrders(string search, string status, DateTime? startDate, DateTime? endDate)
        {
            var orders = _context.Orders.AsQueryable();

            // Lọc theo mã đơn hàng nếu `search` không rỗng
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int orderId))
                {
                    orders = orders.Where(o => o.Order_ID == orderId);
                }
            }

            // Lọc theo trạng thái đơn hàng nếu `status` không rỗng
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            // Lọc theo ngày bắt đầu nếu `startDate` có giá trị
            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate.Value);
            }

            // Lọc theo ngày kết thúc nếu `endDate` có giá trị
            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= endDate.Value);
            }

            var filteredOrders = await orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            if (!filteredOrders.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            // Trả về một partial view nếu có đơn hàng được tìm thấy
            return PartialView("_OrderListPartial", filteredOrders);
        }



        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            // Lấy `User_ID` từ session đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần phải đăng nhập để xem lịch sử đơn hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Tìm Customer_ID dựa trên User_ID
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Dùng Customer_ID để lấy đơn hàng từ bảng Order
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.Customer_ID == customer.Customer_ID)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                return View(new List<Order>());
            }

            return View(orders);

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần phải đăng nhập để hủy đơn hàng." });
            }

            // Tìm đơn hàng theo ID và kiểm tra quyền sở hữu
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Order_ID == id && o.Customer.User_ID == int.Parse(userId));

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            // Kiểm tra trạng thái đơn hàng
            if (order.Status != "Pending" && order.Status != "Đang xử lý")
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng ở trạng thái 'Pending' hoặc 'Đang xử lý'." });
            }

            // Cập nhật trạng thái đơn hàng thành "Đã hủy"
            order.Status = "Đã hủy";
            await _context.SaveChangesAsync();
            Console.WriteLine("Order status updated to 'Đã hủy' for Order_ID: " + id);


            return Json(new { success = true, message = "Đơn hàng đã được hủy thành công." });
        }

        
        // Phương thức để khởi tạo URL thanh toán
        private string GeneratePaymentUrl(dynamic paymentRequest)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("clientId", ClientId),
                new KeyValuePair<string, string>("apiKey", ApiKey),
                new KeyValuePair<string, string>("amount", paymentRequest.amount.ToString()),
                new KeyValuePair<string, string>("currency", paymentRequest.currency),
                new KeyValuePair<string, string>("returnUrl", paymentRequest.returnUrl),
                new KeyValuePair<string, string>("orderId", paymentRequest.orderId),
                new KeyValuePair<string, string>("description", paymentRequest.description)
            };

            // Tạo chữ ký (checksum) theo hướng dẫn của cổng thanh toán
            string checksum = GenerateChecksum(queryParams, ChecksumKey);

            var url = $"{PayosUrl}?{string.Join("&", queryParams.Select(qp => $"{qp.Key}={Uri.EscapeDataString(qp.Value)}"))}&checksum={checksum}";
            return url;
        }

        // Phương thức để tạo checksum từ các tham số và checksumKey
        private string GenerateChecksum(IEnumerable<KeyValuePair<string, string>> parameters, string checksumKey)
        {
            var orderedParams = parameters.OrderBy(p => p.Key).ToList();
            string paramString = string.Join("&", orderedParams.Select(p => $"{p.Key}={p.Value}"));

            // Tạo checksum bằng cách kết hợp paramString và checksumKey với thuật toán SHA256
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(paramString));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // Phương thức xử lý khi người dùng chọn "Thanh toán online"
        [HttpPost]
        public IActionResult InitiateOnlinePayment(string CustomerName, string CustomerPhone, string Province, string District, string Ward, string DetailAddress, string ShippingMethod, string PaymentMethod)
        {
            var currentCustomerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            decimal totalAmount = CalculateTotalAmount(currentCustomerId);

            string returnUrl = Url.Action("PaymentCallback", "Order", null, Request.Scheme);

            var paymentRequest = new
            {
                amount = totalAmount,
                currency = "VND",
                returnUrl = returnUrl,
                orderId = Guid.NewGuid().ToString(),
                description = "Thanh toán đơn hàng tại cửa hàng"
            };

            var paymentUrl = GeneratePaymentUrl(paymentRequest);
            return Json(new { paymentUrl = paymentUrl });
        }



        // Phương thức để xử lý callback sau khi thanh toán
        public IActionResult PaymentCallback(string orderId, string status)
        {
            if (status == "success")
            {
                ViewBag.Message = "Thanh toán thành công!";
            }
            else
            {
                ViewBag.Message = "Thanh toán không thành công!";
            }
            return View();
        }
        private decimal CalculateTotalAmount(int currentCustomerId)
        {
            var cartItems = _context.Carts.Where(c => c.Customer_ID == currentCustomerId).ToList();
            decimal totalAmount = 0;

            foreach (var item in cartItems)
            {
                totalAmount += item.Product.Price * item.Quantity;
            }

            return totalAmount;
        }




    }

}



