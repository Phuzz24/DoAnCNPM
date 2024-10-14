using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DoAnCNPM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DBDienThoaiContext _context;  // Thêm DbContext để truy cập CSDL

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DBDienThoaiContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // Đăng ký người dùng mới (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Đăng ký người dùng mới (POST)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng User mới dựa trên thông tin từ model
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Role = "Khách hàng" // Gán vai trò mặc định là khách hàng
                };

                // Tạo người dùng mới với mật khẩu được băm
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("UpdateProfile", "Account");  // Điều hướng đến trang cập nhật thông tin cá nhân
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        //Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("TrangChu", "Home");
                }
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            }
            return View(model);
        }


        //Đăng xuất
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //Đăng xuất người dùng
            return RedirectToAction("TrangChu", "Home");
        }

        ////Quên mật khẩu
        //[HttpGet]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Tìm người dùng bằng tên đăng nhập
        //        var user = await _userManager.FindByNameAsync(model.Username);
        //        if (user == null)
        //        {
        //            ModelState.AddModelError(string.Empty, "Tên đăng nhập không tồn tại.");
        //            return View();
        //        }

        //        // Gửi email để đặt lại mật khẩu
        //        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = user.Email }, Request.Scheme);

        //        // Gửi email
        //        await _emailSender.SendEmailAsync(user.Email, "Đặt lại mật khẩu", $"Vui lòng nhấn vào liên kết sau để đặt lại mật khẩu: <a href='{resetLink}'>link</a>");

        //        return RedirectToAction("ForgotPasswordConfirmation");
        //    }

        //    return View(model);
        //}

        //[HttpGet]
        //public IActionResult ResetPassword(string token,string email)
        //{
        //    if(token==null || email==null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Liên kết đặt lại mật khẩu không hợp lệ.");

        //    }
        //    return View();
        //}
        //// Đặt lại mật khẩu (POST)
        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Tìm người dùng bằng email
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation");
        //    }
        //    // Đặt lại mật khẩu cho người dùng
        //    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation");
        //    }

        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }

        //    return View(model);
        //}
        //// Xác nhận đặt lại mật khẩu thành công
        //public IActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}
        //// Đăng nhập qua Google (GET)
        //[HttpGet]
        //public IActionResult GoogleLogin()
        //{
        //    var redirectUrl = Url.Action("GoogleResponse", "Account");
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        //    return new ChallengeResult("Google", properties);
        //}

        // Xử lý phản hồi từ Google sau khi đăng nhập (GET)
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return RedirectToAction("TrangChu", "Home");
            }

            var user = new User { UserName = info.Principal.FindFirstValue(ClaimTypes.Email) };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("TrangChu", "Home");
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
