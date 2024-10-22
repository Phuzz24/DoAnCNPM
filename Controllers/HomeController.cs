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

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        public IActionResult TrangChu()
        {
            // Lấy danh sách sản phẩm theo từng category_id
            var iphoneProducts = _database.Products
                .Where(p => p.Category_ID == 1) // 1 là category_id của iPhone
            .ToList();

            var macbookProducts = _database.Products
                .Where(p => p.Category_ID == 3) // 2 là category_id của MacBook
            .ToList();

            var ipadProducts = _database.Products
                .Where(p => p.Category_ID == 4) // 3 là category_id của iPad
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