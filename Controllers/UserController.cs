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

        // Sửa thông tin người dùng
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.User_ID) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("DSNguoiDung");
            }
            return View(user);
        }

        // Xóa người dùng
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("DSNguoiDung");
        }
    }
}
