using DoAnCNPM.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBDienThoaiContext _database;
        public HomeController(DBDienThoaiContext database)
        {
            _database = database;
        }
        private readonly ILogger<HomeController> _logger;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult GioiThieu()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        public IActionResult TrangChu()
        {
            // Lấy danh sách sản phẩm theo từng category_id
            var iphoneProducts = _database.Products
                .Where(p => p.Category_ID == 1) 
            .ToList();

            var macbookProducts = _database.Products
                .Where(p => p.Category_ID == 3) 
            .ToList();

            var ipadProducts = _database.Products
                .Where(p => p.Category_ID == 4) 
                .ToList();

            // Lấy danh sách các sản phẩm bán chạy với số lượng bán > 5 lần

            var topSellingProducts = _database.Products
                .Join(_database.OrderDetails, p => p.Product_ID, od => od.Product_ID, (p, od) => new { Product = p, od.Quantity })
                .GroupBy(x => x.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .Where(x => x.TotalSold > 5) 
                .OrderByDescending(x => x.TotalSold)
                .Take(10) // Lấy 10 sản phẩm bán chạy nhất
                .Select(x => x.Product)
                .ToList();

            // Tạo danh sách các ProductCategoryViewModel để truyền vào View
            var viewModel = new List<ProductCategoryViewModel>
        {
            new ProductCategoryViewModel
            {
                CategoryName = "Điện thoại",
                Products = iphoneProducts
            },
            new ProductCategoryViewModel
            {
                CategoryName = "iPad",
                Products = ipadProducts

            },
            new ProductCategoryViewModel
            {
                CategoryName = "Macbook",
                Products = macbookProducts

            }
        };
            // Gửi danh sách các sản phẩm nổi bật bán chạy nhất
            ViewBag.TopSellingProducts = topSellingProducts;
            return View(viewModel);
        }
        public IActionResult ChiTietSanPham(int id)
        {
            var product = _database.Products.FirstOrDefault(p => p.Product_ID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}