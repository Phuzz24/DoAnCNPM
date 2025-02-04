using DoAnCNPM.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin,Nhân viên")]
        public IActionResult ThongKe()
        {
            var today = DateTime.Today;

            // Đơn hàng hôm nay
            var ordersToday = _context.Orders
                .Where(o => o.OrderDate.Date == today)
                .Count();

            // Doanh thu hôm nay (chỉ tính trạng thái "Đã giao")
            var revenueToday = _context.Orders
                .Where(o => o.Status == "Đã giao" && o.OrderDate.Date == today)
                .Sum(o => o.TotalAmount);

            // Số lượng hàng tồn kho
            var inventoryData = _context.Products
                .Select(p => new
                {
                    NamePro = p.NamePro,
                    Quantity = p.Quantity
                })
                .ToList();

            // Tổng số sản phẩm tồn kho
            var totalInventory = inventoryData.Sum(i => i.Quantity);

            // Số lượng đơn hàng theo trạng thái
            var orderStatusData = _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Doanh thu từng tháng (chỉ tính trạng thái "Đã giao")
            var monthlyRevenue = _context.Orders
                .Where(o => o.Status == "Đã giao")
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            // Sản phẩm bán chạy nhất
            var topSellingProducts = _context.OrderDetails
                .GroupBy(od => od.Product.NamePro)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(5)
                .ToList();

            ViewBag.OrdersToday = ordersToday;
            ViewBag.RevenueToday = revenueToday;
            ViewBag.Inventory = inventoryData;
            ViewBag.TotalInventory = totalInventory;
            ViewBag.OrderStatusData = orderStatusData;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.TopSellingProducts = topSellingProducts;

            return View();
        }




    }
}
