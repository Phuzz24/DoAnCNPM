using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using PagedList.Core;
using PagedList.Core.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using DoAnCNPM.Helpers;
using DoAnCNPM.Filters;

namespace DoAnCNPM.Controllers
{

    public class ProductController : Controller
    {
        public readonly DBDienThoaiContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Khai báo _webHostEnvironment
        private readonly IAntiforgery _antiforgery;


        public ProductController(DBDienThoaiContext context, IWebHostEnvironment webHostEnvironment, IAntiforgery antiforgery)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _antiforgery = antiforgery;
        }
        //Trang sản phẩm cho khách hàng xem
        public IActionResult SanPham(string searchTerm = "", int? brandId = null, int? categoryId = null, string priceRange = "")
        {
            var products = _context.Products.AsQueryable();

            // Áp dụng tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.NamePro.Contains(searchTerm));
            }

            // Các bộ lọc khác (nếu có)
            if (brandId.HasValue)
            {
                products = products.Where(p => p.Brand_ID == brandId.Value);
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.Category_ID == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceParts = priceRange.Split('-');
                if (priceParts.Length == 2 && int.TryParse(priceParts[0], out int minPrice) && int.TryParse(priceParts[1], out int maxPrice))
                {
                    products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
                else if (priceParts.Length == 1 && priceRange == "30000000")
                {
                    products = products.Where(p => p.Price >= 30000000);
                }
            }

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();

            return View(products.ToList());
        }

        [HttpGet]
        public IActionResult Search(string Timkiem)
        {
            var products = _context.Products
            .Where(p => p.NamePro.Contains(Timkiem))
            .ToList();
            return View("SearchResults", products); // Hiển thị kết quả tìm kiếm
        }

        //Chi tiết sản phẩm về Trang Sản phẩm
        public async Task<IActionResult> ChiTietSanPham(int id)
        {
            var product = await _context.Products
                .Include(p => p.Feedbacks)
                .ThenInclude(f => f.Customer)
                .FirstOrDefaultAsync(p => p.Product_ID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // Phương thức để thêm bình luận mới
        [HttpPost]
        [Authorize] // Chỉ cho phép người dùng đã đăng nhập thêm bình luận
        public async Task<IActionResult> AddComment(int productId, string comment)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Lấy ID của người dùng từ Claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Chuyển đổi User_ID từ string sang int
                    if (int.TryParse(userId, out int userIdParsed))
                    {
                        // Tìm khách hàng trong cơ sở dữ liệu
                        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == userIdParsed);

                        if (customer != null)
                        {
                            // Tạo đối tượng bình luận mới
                            var feedback = new Feedback
                            {
                                Customer_ID = customer.Customer_ID,
                                Product_ID = productId,
                                Comment = comment,
                                FeedbackDate = DateTime.Now
                            };

                            // Thêm bình luận vào DbContext và lưu vào cơ sở dữ liệu
                            _context.Feedbacks.Add(feedback);
                            await _context.SaveChangesAsync(); 

                            // Sau khi thêm bình luận thành công, chuyển hướng về trang chi tiết sản phẩm
                            return RedirectToAction("ChiTietSanPham", new { id = productId });
                        }
                    }
                }
            }

            // Nếu có lỗi hoặc không đăng nhập, chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }


        //Xóa bình luận
        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> DeleteComment(int commentId, int productId)
        {
            // Lấy bình luận theo ID
            var feedback = await _context.Feedbacks.FindAsync(commentId);

            if (feedback != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của người dùng đăng nhập

                // Kiểm tra nếu người dùng là chủ nhân của bình luận
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));
                if (customer != null && feedback.Customer_ID == customer.Customer_ID)
                {
                    _context.Feedbacks.Remove(feedback); // Xóa bình luận
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ChiTietSanPham", new { id = productId });
                }
            }

            // Nếu không thể xóa (bình luận không tồn tại hoặc không phải của người dùng), chuyển hướng về trang chi tiết sản phẩm
            return RedirectToAction("ChiTietSanPham", new { id = productId });
        }


        //Trang quản lí sản phẩm
        [Authorize(Roles = "Nhân viên,Admin")]
        public IActionResult DSSanPham(string searchTerm = "", string priceRange = "", int page = 1, int pageSize = 10)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PriceRange = priceRange;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var products = _context.Products.AsQueryable();

            // Lọc sản phẩm theo từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.NamePro.Contains(searchTerm));
            }

            // Lọc sản phẩm theo khoảng giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceParts = priceRange.Split('-');
                if (priceParts.Length == 2 && int.TryParse(priceParts[0], out int minPrice) && int.TryParse(priceParts[1], out int maxPrice))
                {
                    products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
                else if (priceParts.Length == 1 && priceRange == "30000000")
                {
                    products = products.Where(p => p.Price >= 30000000);
                }
            }

            // Tổng số sản phẩm
            int totalItems = products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Phân trang
            var paginatedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(paginatedProducts);
        }


        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> CreatePost()
        {
            try
            {
                // Lấy dữ liệu từ form
                string name = Request.Form["NamePro"];
                string brandId = Request.Form["Brand_ID"];
                string categoryId = Request.Form["Category_ID"];
                string priceStr = Request.Form["Price"];
                string quantityStr = Request.Form["Quantity"];
                string description = Request.Form["Description"];
                IFormFile imageFile = Request.Form.Files["ImageFile"];

                // Kiểm tra tính hợp lệ của dữ liệu
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(brandId) || string.IsNullOrEmpty(categoryId) ||
                    string.IsNullOrEmpty(priceStr) || string.IsNullOrEmpty(quantityStr) || 
                    string.IsNullOrEmpty(description) || imageFile == null)
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ các thông tin!";
                    ViewBag.Brands = _context.Brands.ToList();
                    ViewBag.Categories = _context.Categories.ToList();
                    return View("Create");
                }

                if (!decimal.TryParse(priceStr, out decimal price) || price < 0)
                {
                    ViewBag.Error = "Giá không hợp lệ!";
                    ViewBag.Brands = _context.Brands.ToList();
                    ViewBag.Categories = _context.Categories.ToList();
                    return View("Create");
                }

                if (!int.TryParse(quantityStr, out int quantity) || quantity < 0)
                {
                    ViewBag.Error = "Số lượng không hợp lệ!";
                    ViewBag.Brands = _context.Brands.ToList();
                    ViewBag.Categories = _context.Categories.ToList();
                    return View("Create");
                }

                // Kiểm tra thư mục lưu ảnh và lưu file ảnh
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "HinhAnh");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Tạo thư mục nếu chưa tồn tại
                }

                // Tạo tên file duy nhất để tránh trùng lặp
                string fileName = Path.GetFileName(imageFile.FileName);
                string relativePath = Path.Combine("HinhAnh", fileName).Replace("\\", "/"); // Thay thế dấu "\" thành "/"
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Tạo sản phẩm mới
                var product = new Product
                {
                    NamePro = name,
                    Brand_ID = int.Parse(brandId),
                    Category_ID = int.Parse(categoryId),
                    Price = price,
                    Quantity = quantity,
                    Description = description,
                    Image = "/" + relativePath // Đảm bảo lưu đường dẫn như yêu cầu
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("DSSanPham");
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình thêm sản phẩm. Chi tiết lỗi: " + innerExceptionMessage;
                ViewBag.Brands = _context.Brands.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View("Create");
            }
        }


        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound(); // Trả về 404 nếu sản phẩm không tồn tại
            }

            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(product); // Trả về view Edit với model sản phẩm
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Product_ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Brands = _context.Brands.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }

            try
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    ViewBag.Error = "Không tìm thấy sản phẩm để cập nhật.";
                    return View(product);
                }

                // Kiểm tra nếu có ảnh mới được tải lên
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string relativePath = Path.Combine("HinhAnh", fileName).Replace("\\", "/");
                    string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Cập nhật đường dẫn ảnh mới
                    existingProduct.Image = "/" + relativePath;
                }
                else
                {
                    // Nếu không có ảnh mới, giữ nguyên đường dẫn ảnh cũ
                    product.Image = existingProduct.Image;
                }

                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.NamePro = product.NamePro;
                existingProduct.Brand_ID = product.Brand_ID;
                existingProduct.Category_ID = product.Category_ID;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Description = product.Description;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("DSSanPham");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra trong quá trình cập nhật sản phẩm. Chi tiết: " + ex.Message;
                ViewBag.Brands = _context.Brands.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        //Chi tiết sản phẩm
        [Authorize(Roles = "Admin")]

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Where(p => p.Product_ID == id)
                .Select(p => new
                {
                    Name = p.NamePro,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    Image = p.Image
                })
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại." });
            }

            return Json(product); // Trả về dữ liệu JSON
        }

        //Xóa nhiều sản phẩm cùng lúc
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Json(new { success = false, message = "Không có sản phẩm nào được chọn để xóa." });
                }

                var productsToDelete = await _context.Products.Where(p => ids.Contains(p.Product_ID)).ToListAsync();

                if (productsToDelete.Count > 0)
                {
                    _context.Products.RemoveRange(productsToDelete);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Đã xóa các sản phẩm được chọn thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm nào để xóa." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }

}



