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

        // Phương thức loại bỏ dấu tiếng Việt
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
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

            // Loại bỏ dấu và rút gọn `Username`
            string baseUsername = RemoveDiacritics(staff.NameStaff.ToLower().Replace(" ", ""));
            string uniqueUsername = $"{baseUsername}_{new Random().Next(1000, 9999)}"; // Tạo số ngẫu nhiên 4 chữ số

            // Tạo một User mới với các giá trị mặc định
            var newUser = new User
            {
                Username = uniqueUsername, // Giá trị mặc định, ngắn gọn và không dấu
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
                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";

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
                TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";

                return RedirectToAction("DSNhanVien"); // Chuyển hướng về danh sách nhân viên sau khi cập nhật thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(staff);
            }
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
                    var staff = await _context.Staffs.FindAsync(id);
                    if (staff == null)
                    {
                        return Json(new { success = false, message = "Nhân viên không tồn tại." });
                    }

                    // Xóa các bản ghi liên quan trong bảng Customer, Staff, Admin nếu tồn tại
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.User_ID == id);

                    if (user != null) _context.Users.Remove(user);

                    // Xóa người dùng
                    _context.Staffs.Remove(staff);

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
