using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoAnCNPM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public CustomerController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Tìm kiếm nhân viên
        public async Task<IActionResult> DSKhachHang(string searchTerm)
        {
            var cus = string.IsNullOrEmpty(searchTerm) ? _context.Customers :
                _context.Customers.Where(s => s.NameCus.Contains(searchTerm) || s.EmailCus.Contains(searchTerm));
            return View(await cus.ToListAsync());
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
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(customer);
            }

            // Loại bỏ dấu và rút gọn `Username`
            string baseUsername = RemoveDiacritics(customer.NameCus.ToLower().Replace(" ", ""));
            string uniqueUsername = $"{baseUsername}_{new Random().Next(1000, 9999)}"; // Tạo số ngẫu nhiên 4 chữ số
            // Tạo một User mới với các giá trị mặc định
            var newUser = new User
            {
                Username = uniqueUsername, // Giá trị mặc định
                Password = "default_password", // Giá trị mặc định
                Role = "Khách hàng" // Vai trò mặc định
            };

            // Thêm User vào cơ sở dữ liệu
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Gán User_ID vừa tạo cho Staff
            customer.User_ID = newUser.User_ID;

            try
            {
                // Thêm Staff vào cơ sở dữ liệu
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync(); // Lưu Staff
                TempData["SuccessMessage"] = "Thêm khách hàng thành công!";

                return RedirectToAction("DSKhachHang"); // Chuyển hướng về danh sách nhân viên sau khi thêm thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(customer);
            }
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm kiếm Staff theo ID
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Lấy thông tin User liên kết với Staff (nếu cần để hiển thị hoặc chỉnh sửa)
            customer.User = await _context.Users.FindAsync(customer.User_ID);

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Customer_ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(customer);
            }

            try
            {
                // Tìm kiếm Staff trong cơ sở dữ liệu
                var existingStaff = await _context.Customers.FindAsync(id);
                if (existingStaff == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin của Staff
                existingStaff.NameCus = customer.NameCus;
                existingStaff.AddressCus = customer.AddressCus;
                existingStaff.PhoneCus = customer.PhoneCus;
                existingStaff.EmailCus = customer.EmailCus;

                // Cập nhật thay đổi vào database
                _context.Customers.Update(existingStaff);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật khách hàng thành công!";

                return RedirectToAction("DSKhachHang"); // Chuyển hướng về danh sách nhân viên sau khi cập nhật thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(customer);
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
                    var customer = await _context.Customers.FindAsync(id);
                    if (customer == null)
                    {
                        return Json(new { success = false, message = "Khách hàng không tồn tại." });
                    }

                    // Xóa các bản ghi liên quan trong bảng Customer, Staff, Admin nếu tồn tại
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.User_ID == id);

                    if (user != null) _context.Users.Remove(user);

                    // Xóa người dùng
                    _context.Customers.Remove(customer);

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
