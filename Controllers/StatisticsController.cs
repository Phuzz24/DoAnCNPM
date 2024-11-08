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

        public StatisticsController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Tính doanh thu theo khoảng thời gian (1 ngày, 7 ngày, 30 ngày)
        public async Task<IActionResult> Revenue(int days)
        {
            DateTime startDate = DateTime.Now.AddDays(-days);
            var revenueData = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Đã giao")
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalAmount ?? 0)
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return Json(revenueData);
        }

        // Trả về danh sách Top 10 khách hàng mua hàng nhiều nhất
        public async Task<IActionResult> TopCustomers()
        {
            var topCustomers = await _context.Orders
                .Where(o => o.Status == "Đã giao")
                .GroupBy(o => o.Customer_ID)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    CustomerName = _context.Customers.FirstOrDefault(c => c.Customer_ID == g.Key).NameCus,
                    TotalSpent = g.Sum(o => o.TotalAmount ?? 0)
                })
                .OrderByDescending(g => g.TotalSpent)
                .Take(10)
                .ToListAsync();

            return Json(topCustomers);
        }

        // Thống kê Top 10 sản phẩm bán chạy nhất và còn tồn kho
        public async Task<IActionResult> TopProductInventory()
        {
            var productInventory = await _context.Products
                .Select(p => new
                {
                    ProductName = p.NamePro,
                    QuantitySold = p.OrderDetails.Sum(od => od.Quantity),
                    QuantityInStock = p.Quantity
                })
                .OrderByDescending(p => p.QuantitySold) // Sắp xếp theo số lượng bán
                .Take(10) // Lấy top 10 sản phẩm
                .ToListAsync();

            return Json(productInventory);
        }

        public IActionResult ThongKe()
        {
            return View();
        }
    }
}
