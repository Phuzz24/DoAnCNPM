using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoAnCNPM.Controllers
{
    public class StaffController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public StaffController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Tìm kiếm nhân viên
        public async Task<IActionResult> DSNhanVien(string searchTerm)
        {
            var staffs = string.IsNullOrEmpty(searchTerm) ? _context.Staffs :
                _context.Staffs.Where(s => s.NameStaff.Contains(searchTerm) || s.EmailStaff.Contains(searchTerm));
            return View(await staffs.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(staff);
            }

            string uniqueUsername = $"{staff.NameStaff.ToLower().Replace(" ", "")}_{DateTime.Now.Ticks}";

            // Tạo một User mới với các giá trị mặc định
            var newUser = new User
            {
                Username = uniqueUsername, // Giá trị mặc định
                Password = "default_password", // Giá trị mặc định
                Role = "Nhân viên" // Vai trò mặc định
            };

            // Thêm User vào cơ sở dữ liệu
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Gán User_ID vừa tạo cho Staff
            staff.User_ID = newUser.User_ID;

            try
            {
                // Thêm Staff vào cơ sở dữ liệu
                _context.Staffs.Add(staff);
                await _context.SaveChangesAsync(); // Lưu Staff
                return RedirectToAction("DSNhanVien"); // Chuyển hướng về danh sách nhân viên sau khi thêm thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(staff);
            }
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm kiếm Staff theo ID
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            // Lấy thông tin User liên kết với Staff (nếu cần để hiển thị hoặc chỉnh sửa)
            staff.User = await _context.Users.FindAsync(staff.User_ID);

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.Staff_ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(staff);
            }

            try
            {
                // Tìm kiếm Staff trong cơ sở dữ liệu
                var existingStaff = await _context.Staffs.FindAsync(id);
                if (existingStaff == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin của Staff
                existingStaff.NameStaff = staff.NameStaff;
                existingStaff.AddressStaff = staff.AddressStaff;
                existingStaff.PhoneStaff = staff.PhoneStaff;
                existingStaff.EmailStaff = staff.EmailStaff;

                // Cập nhật thay đổi vào database
                _context.Staffs.Update(existingStaff);
                await _context.SaveChangesAsync();

                return RedirectToAction("DSNhanVien"); // Chuyển hướng về danh sách nhân viên sau khi cập nhật thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(staff);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm kiếm Staff theo ID
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm kiếm Staff cần xóa
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            // Kiểm tra xem User có liên kết với Staff không
            var user = await _context.Users.FindAsync(staff.User_ID);

            try
            {
                // Xóa Staff
                _context.Staffs.Remove(staff);

                // Nếu không có liên kết khác với User này, xóa luôn User
                if (user != null && !_context.Staffs.Any(s => s.User_ID == user.User_ID))
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("DSNhanVien"); // Chuyển hướng về danh sách nhân viên sau khi xóa thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi xóa: {ex.Message}";
                return View(staff); // Trả lại View với thông báo lỗi nếu có lỗi xảy ra
            }
        }

    }
}
