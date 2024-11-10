using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoAnCNPM.Controllers
{
    public class UserController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public UserController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Tìm kiếm người dùng
        public async Task<IActionResult> DSNguoiDung(string searchTerm)
        {
            var users = string.IsNullOrEmpty(searchTerm) ? _context.Users :
                _context.Users.Where(u => u.Username.Contains(searchTerm) || u.Role.Contains(searchTerm));
            return View(await users.ToListAsync());
        }

        // Thêm mới người dùng
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("DSNguoiDung");
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm kiếm User theo ID
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng để cập nhật.";
                return RedirectToAction("Error");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("User_ID, Username, Password, Role")] User user)
        {
            if (id != user.User_ID)
            {
                TempData["ErrorMessage"] = "ID người dùng không hợp lệ.";
                return RedirectToAction("Error");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(user);
            }

            try
            {
                // Lấy thông tin User từ cơ sở dữ liệu
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng để cập nhật.";
                    return RedirectToAction("Error");
                }

                // Cập nhật thông tin của User
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;

                // Lưu thay đổi vào database
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                return RedirectToAction("DSNguoiDung");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(user);
            }
        }


        // Xử lý hiển thị trang lỗi
        public IActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF Protection
        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tìm user theo ID
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return Json(new { success = false, message = "Người dùng không tồn tại." });
                    }

                    // Xóa các bản ghi liên quan trong bảng Customer, Staff, Admin nếu tồn tại
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.User_ID == id);
                    var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.User_ID == id);
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.User_ID == id);

                    if (customer != null) _context.Customers.Remove(customer);
                    if (staff != null) _context.Staffs.Remove(staff);
                    if (admin != null) _context.Admins.Remove(admin);

                    // Xóa người dùng
                    _context.Users.Remove(user);

                    // Lưu thay đổi và xác nhận giao dịch
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Hoàn tác giao dịch nếu có lỗi
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Có lỗi xảy ra khi xóa đối tượng." });
                }
            }
        }

        
    }
}
