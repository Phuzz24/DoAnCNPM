using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace DoAnCNPM.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public AccountController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPost()
        {
            try
            {
                // Lấy dữ liệu từ form
                string username = Request.Form["Username"];
                string email = Request.Form["Email"];
                string password = Request.Form["Password"];
                string confirmPassword = Request.Form["ConfirmPassword"];

                // Kiểm tra tính hợp lệ
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ các thông tin!";
                    return View("Register");  // Trả về View Register.cshtml nếu có lỗi
                }

                if (password.Length < 6)
                {
                    ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự!";
                    return View("Register");
                }

                if (password != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp!";
                    return View("Register");
                }

                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (existingUser != null)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                    return View("Register");
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingEmail = await _context.Customers.FirstOrDefaultAsync(c => c.EmailCus == email);

                if (existingEmail != null)
                {
                    ViewBag.Error = "Email đã tồn tại.";
                    return View("Register");
                }
                //if(existingUser && existingEmail !=null)
                //{
                //    ViewBag.Error = "Tên đăng nhập và Email đã tồn tại.";
                //    return View(Register);
                //}    

                // Tạo người dùng mới
                var user = new User
                {
                    Username = username,
                    Password = password,
                    Role = "Khách hàng"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var customer = new Customer
                {
                    User_ID = user.User_ID,
                    NameCus = "Khách hàng chưa cập nhật", // Giá trị mặc định
                    AddressCus = "Chưa có địa chỉ",       // Giá trị mặc định
                    PhoneCus = "0000000000",              // Giá trị mặc định
                    EmailCus = email                      // Sử dụng email từ form đăng ký
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết và hiển thị thông báo lỗi chung
                ViewBag.Error = "Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại.";
                return View("Register");
            }
        }





        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPost()
        {
            string username = Request.Form["Username"];
            string password = Request.Form["Password"];
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View("Login");
            }    
            if(password.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải từ 6 kí tự trở lên.";
                return View("Login");
            }    

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username); // Lưu tên vào session
                HttpContext.Session.SetInt32("UserID", user.User_ID); // Lưu ID vào session
                TempData["SuccessMessage"] = "Đăng nhập thành công!";

                // Đăng nhập người dùng
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.User_ID.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (user.Role == "Khách hàng")
                {
                    return RedirectToAction("TrangChu", "Home");
                }
                else if (user.Role == "Admin" || user.Role =="Nhân viên")
                {
                    return RedirectToAction("DSSanPham", "Product");
                }
                return RedirectToAction("TrangChu", "Home");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác.";
            return View("Login");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Đăng xuất và xóa cookie xác thực
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }





        [HttpGet]
        public IActionResult EditInfo()
        {
            // Kiểm tra xem session có Username không
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Người dùng không tồn tại hoặc chưa đăng nhập.");
            }

            // Tìm người dùng trong cơ sở dữ liệu dựa trên Username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Tìm khách hàng liên kết với User
            var customer = _context.Customers.FirstOrDefault(c => c.User_ID == user.User_ID);

            // Nếu khách hàng chưa có thông tin, tạo đối tượng mới với giá trị trống để hiển thị
            if (customer == null)
            {
                customer = new Customer
                {
                    NameCus = "Tên khách hàng mặc định", // Gán giá trị mặc định
                    AddressCus = "Địa chỉ mặc định",    // Gán giá trị mặc định
                    PhoneCus = "0000000000",             // Gán số điện thoại mặc định
                    EmailCus = user.Username             // Sử dụng username làm email nếu chưa có thông tin
                };
            }
            else
            {
                // Kiểm tra và gán giá trị mặc định nếu có trường nào null
                customer.NameCus = customer.NameCus ?? "Tên khách hàng mặc định";
                customer.AddressCus = customer.AddressCus ?? "Địa chỉ mặc định";
                customer.PhoneCus = customer.PhoneCus ?? "0000000000";
                customer.EmailCus = customer.EmailCus ?? user.Username;
            }

            // Tạo view model để truyền dữ liệu tới view
            var model = new UserInfoViewModel
            {
                Username = user.Username, // Không cho chỉnh sửa
                NameCus = customer.NameCus,
                AddressCus = customer.AddressCus,
                PhoneCus = customer.PhoneCus,
                EmailCus = customer.EmailCus
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult EditInfoPost(UserInfoViewModel model)
        {
            // Kiểm tra xem session có Username không
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Người dùng không tồn tại hoặc chưa đăng nhập.");
            }

            // Tìm người dùng dựa trên Username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Tìm khách hàng liên kết với User
            var customer = _context.Customers.FirstOrDefault(c => c.User_ID == user.User_ID);
            if (customer == null)
            {
                return NotFound("Không tìm thấy thông tin khách hàng.");
            }

            // Cập nhật thông tin khách hàng
            customer.NameCus = model.NameCus;
            customer.AddressCus = model.AddressCus;
            customer.PhoneCus = model.PhoneCus;
            customer.EmailCus = model.EmailCus;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("EditInfo");
        }

        // GET: Hiển thị form đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Xử lý đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy User hiện tại từ cơ sở dữ liệu (ví dụ: dựa trên Username từ session hoặc context)
            var username = User.Identity.Name; // Giả sử đã có username trong session
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View(model);
            }

            // Kiểm tra mật khẩu hiện tại
            if (user.Password != model.CurrentPassword) // So sánh trực tiếp (nếu đã mã hóa, cần giải mã hoặc mã hóa tương tự để so sánh)
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            // Đổi mật khẩu
            user.Password = model.NewPassword; // Lưu ý: Nếu mật khẩu cần mã hóa, hãy áp dụng mã hóa tại đây
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("EditInfo", "Account"); // Chuyển hướng sau khi thành công
        }




    }
}
