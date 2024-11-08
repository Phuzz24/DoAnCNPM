using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace DoAnCNPM.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DBDienThoaiContext _context;
        public PaymentController(DBDienThoaiContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            // Cấu hình HttpClient
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "fa32a6e4-ea66-4d96-a6dd-852b3ecd7cab"); // Thay YOUR_API_KEY bằng API Key của bạn

            // Thông tin yêu cầu thanh toán
            var paymentRequest = new
            {
                amount = order.TotalAmount,
                currency = "VND",
                orderId = order.Order_ID,
                returnUrl = Url.Action("PaymentCallback", "Payment", new { orderId = order.Order_ID }, Request.Scheme)
            };

            var content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.payos.vn/v1/payments", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(responseData);
                return Redirect(paymentResponse.PaymentUrl); // Chuyển hướng người dùng tới trang thanh toán PayOS
            }

            return View("Error", "Không thể tạo yêu cầu thanh toán");
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallback(int orderId, string status, string transactionId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            if (status == "success")
            {
                // Cập nhật trạng thái đơn hàng là đã thanh toán
                ViewBag.Message = "Thanh toán thành công!";
            }
            else
            {
                ViewBag.Message = "Thanh toán không thành công!";
            }

            // Lưu Transaction ID từ PayOS nếu cần
            order.TrackingNumber = transactionId;

            await _context.SaveChangesAsync();

            return View("PaymentResult", order); // Hiển thị thông tin thanh toán cho người dùng
        }
    }
}
