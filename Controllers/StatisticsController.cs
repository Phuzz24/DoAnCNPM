using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCNPM.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly DBDienThoaiContext _context;
        private static int _visitCount = 0; // Biến đếm số lần truy cập

        public StatisticsController(DBDienThoaiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ThongKe()
        {
            // Tăng số lần truy cập mỗi khi trang thống kê được mở
            _visitCount++;

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-7);
            var startOfMonth = today.AddDays(-30);

            // Lọc doanh thu từ các đơn hàng đã giao và đã thanh toán
            var revenueToday = await _context.Orders
                .Where(o => o.OrderDate.Date == today && o.Status == "Đã giao" && o.PaymentStatus == "Đã thanh toán")
                .SumAsync(o => o.TotalAmount);

            var revenueWeek = await _context.Orders
                .Where(o => o.OrderDate >= startOfWeek && o.Status == "Đã giao" && o.PaymentStatus == "Đã thanh toán")
                .SumAsync(o => o.TotalAmount);

            var revenueMonth = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth && o.Status == "Đã giao" && o.PaymentStatus == "Đã thanh toán")
                .SumAsync(o => o.TotalAmount);

            var ordersToday = await _context.Orders
                .CountAsync(o => o.OrderDate.Date == today && o.Status == "Đã giao" && o.PaymentStatus == "Đã thanh toán");

            var ordersWeek = await _context.Orders
                .CountAsync(o => o.OrderDate >= startOfWeek && o.Status == "Đã giao" && o.PaymentStatus == "Đã thanh toán");

            var ordersMonth = await _context.Orders
                .CountAsync(o => o.OrderDate >= startOfMonth && o.Status == "Đã giao" && o.PaymentStatus == "Đã thanh toán");

            // Sản phẩm bán chạy
            var topProducts = await _context.OrderDetails
                .Where(od => od.Order.Status == "Đã giao" && od.Order.PaymentStatus == "Đã thanh toán")
                .GroupBy(od => od.Product_ID)
                .Select(g => new
                {
                    ProductId = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    ProductName = g.FirstOrDefault().Product.NamePro
                })
                .OrderByDescending(g => g.QuantitySold)
                .Take(5)
                .ToListAsync();

            //// Người dùng thường xuyên truy cập
            //var frequentUsers = await _context.Users
            //    .OrderByDescending(u => u.Customer.Count)
            //    .Take(5)
            //    .Select(u => new { u.Username })
            //    .ToListAsync();

            // Gửi dữ liệu đến View
            ViewBag.RevenueToday = revenueToday;
            ViewBag.RevenueWeek = revenueWeek;
            ViewBag.RevenueMonth = revenueMonth;
            ViewBag.OrdersToday = ordersToday;
            ViewBag.OrdersWeek = ordersWeek;
            ViewBag.OrdersMonth = ordersMonth;
            ViewBag.TopProducts = topProducts;
            //ViewBag.FrequentUsers = frequentUsers;
            ViewBag.VisitCount = _visitCount; // Truyền số lần truy cập vào View

            return View();
        }
    }
}
