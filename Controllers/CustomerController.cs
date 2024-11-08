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

            string uniqueUsername = $"{customer.NameCus.ToLower().Replace(" ", "")}_{DateTime.Now.Ticks}";

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

                return RedirectToAction("DSKhachHang"); // Chuyển hướng về danh sách nhân viên sau khi cập nhật thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(customer);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm kiếm Staff theo ID
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm kiếm Staff cần xóa
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Kiểm tra xem User có liên kết với Staff không
            var user = await _context.Users.FindAsync(customer.User_ID);

            try
            {
                // Xóa Staff
                _context.Customers.Remove(customer);

                // Nếu không có liên kết khác với User này, xóa luôn User
                if (user != null && !_context.Customers.Any(s => s.User_ID == user.User_ID))
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("DSKhachHang"); // Chuyển hướng về danh sách nhân viên sau khi xóa thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi xóa: {ex.Message}";
                return View(customer); // Trả lại View với thông báo lỗi nếu có lỗi xảy ra
            }
        }
        }
    }
