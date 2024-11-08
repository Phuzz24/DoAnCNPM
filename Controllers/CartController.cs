using DoAnCNPM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DoAnCNPM.Controllers
{
    public class CartController : Controller
    {
        private readonly DBDienThoaiContext _context;
        public CartController(DBDienThoaiContext context)
        {
            _context = context;
        }
        // Hiển thị giỏ hàng
        [Authorize]
        public async Task<IActionResult> GioHang()
        {
            // Kiểm tra session của người dùng
            var username = HttpContext.Session.GetString("Username");
            var userId = HttpContext.Session.GetInt32("UserID");

            if (username == null || userId == null)
            {
                // Nếu session bị mất hoặc hết hạn, yêu cầu người dùng đăng nhập lại
                TempData["ErrorMessage"] = "Phiên làm việc của bạn đã hết hạn, vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == userId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Login", "Account");
            }

            // Tiếp tục logic xử lý giỏ hàng
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.Customer_ID == customer.Customer_ID)
                .ToListAsync();

            return View(cartItems);
        }



        // Thêm sản phẩm vào giỏ hàng
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần phải đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("TrangChu", "Home");
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.Customer_ID == customer.Customer_ID && c.Product_ID == productId);

            if (existingCartItem != null)
            {
                if (existingCartItem.Quantity + 1 > product.Quantity)
                {
                    TempData["ErrorMessage"] = $"Số lượng yêu cầu vượt quá số lượng tồn kho hiện có. Hiện có {product.Quantity} sản phẩm.";
                    return RedirectToAction("GioHang");
                }

                existingCartItem.Quantity += 1;
                _context.Carts.Update(existingCartItem);
            }
            else
            {
                if (product.Quantity < 1)
                {
                    TempData["ErrorMessage"] = "Sản phẩm này hiện đã hết hàng.";
                    return RedirectToAction("GioHang");
                }

                var cartItem = new Cart
                {
                    Customer_ID = customer.Customer_ID,
                    Product_ID = productId,
                    Quantity = 1,
                    AddedAt = DateTime.Now
                };

                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("GioHang");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("GioHang");
            }

            // Kiểm tra số lượng tồn kho của sản phẩm
            if (quantity > product.Quantity)
            {
                TempData["ErrorMessage"] = $"Số lượng yêu cầu vượt quá số lượng tồn kho hiện có. Hiện có {product.Quantity} sản phẩm.";
                return RedirectToAction("GioHang");
            }

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.Customer_ID == customer.Customer_ID && c.Product_ID == productId);

            if (cartItem != null)
            {
                if (quantity > 0)
                {
                    cartItem.Quantity = quantity;
                    _context.Carts.Update(cartItem);
                }
                else
                {
                    _context.Carts.Remove(cartItem);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("GioHang");
        }







        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == int.Parse(userId));
            if (customer == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            }

            // Tìm sản phẩm trong giỏ hàng dựa trên productId
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.Customer_ID == customer.Customer_ID && c.Product_ID == productId);

            if (cartItem != null)
            {
                // Xóa sản phẩm khỏi giỏ hàng
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng!" });
            }
            else
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }
        }

    }
}
