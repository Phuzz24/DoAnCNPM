using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCNPM.Controllers
{
    public class AdminController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public AdminController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách Admins
        public IActionResult DSAdmin(string searchTerm)
        {
            var admins = _context.Admins.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                admins = admins.Where(a => a.AdminName.Contains(searchTerm) || a.EmailAd.Contains(searchTerm));
            }
            return View(admins.ToList());
        }

        // Thêm Admin mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
                return View(admin);
            }

            // Tạo User cho Admin
            var newUser = new User
            {
                Username = "default_username", // Giá trị mặc định
                Password = "default_password", // Giá trị mặc định
                Role = "Admin" // Vai trò mặc định
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            admin.User_ID = newUser.User_ID;
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction("DSAdmin");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm kiếm Staff theo ID
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            // Lấy thông tin User liên kết với Staff (nếu cần để hiển thị hoặc chỉnh sửa)
            admin.User = await _context.Users.FindAsync(admin.User_ID);

            return View(admin);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Admin admin)
        {
            if (id != admin.Admin_ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(admin);
            }

            try
            {
                // Tìm kiếm Staff trong cơ sở dữ liệu
                var existingStaff = await _context.Admins.FindAsync(id);
                if (existingStaff == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin của Staff
                existingStaff.AdminName = admin.AdminName;
                existingStaff.AddressAd = admin.AddressAd;
                existingStaff.PhoneAd = admin.PhoneAd;
                existingStaff.EmailAd = admin.EmailAd;

                // Cập nhật thay đổi vào database
                _context.Admins.Update(existingStaff);
                await _context.SaveChangesAsync();

                return RedirectToAction("DSAdmin"); // Chuyển hướng về danh sách nhân viên sau khi cập nhật thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(admin);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm kiếm Staff theo ID
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm kiếm Staff cần xóa
            var ad = await _context.Admins.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            // Kiểm tra xem User có liên kết với Staff không
            var user = await _context.Users.FindAsync(ad.User_ID);

            try
            {
                // Xóa Staff
                _context.Admins.Remove(ad);

                // Nếu không có liên kết khác với User này, xóa luôn User
                if (user != null && !_context.Staffs.Any(s => s.User_ID == user.User_ID))
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("DSAdmin"); // Chuyển hướng về danh sách nhân viên sau khi xóa thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi xóa: {ex.Message}";
                return View(ad); // Trả lại View với thông báo lỗi nếu có lỗi xảy ra
            }
        }
    }
}
