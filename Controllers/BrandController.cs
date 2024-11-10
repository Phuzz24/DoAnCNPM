using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace DoAnCNPM.Controllers
{
    public class BrandController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public BrandController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Danh sách thương hiệu
        public async Task<IActionResult> DSBrand(string searchTerm)
        {
            var brands = string.IsNullOrEmpty(searchTerm)
                ? _context.Brands
                : _context.Brands.Where(b => b.BrandName.Contains(searchTerm));
            return View(await brands.ToListAsync());
        }

        // Thêm mới - GET
        [HttpGet]
        public IActionResult Create() => View();

        // Thêm mới - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm thương hiệu thành công!";
                return RedirectToAction(nameof(DSBrand));
            }
            return View(brand);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm thương hiệu dựa trên ID
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                TempData["ErrorMessage"] = "ID thương hiệu không hợp lệ.";
                return RedirectToAction("DSBrand");
            }

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand)
        {
            if (id != brand.Brand_ID)
            {
                TempData["ErrorMessage"] = "ID thương hiệu không hợp lệ.";
                return RedirectToAction("DSBrand");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(brand);
            }

            try
            {
                // Tìm thương hiệu trong cơ sở dữ liệu
                var existingBrand = await _context.Brands.FindAsync(id);
                if (existingBrand == null)
                {
                    TempData["ErrorMessage"] = "Thương hiệu không tồn tại.";
                    return RedirectToAction("DSBrand");
                }

                // Cập nhật thông tin thương hiệu
                existingBrand.BrandName = brand.BrandName;

                // Cập nhật thay đổi vào database
                _context.Brands.Update(existingBrand);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thương hiệu thành công!";

                return RedirectToAction("DSBrand"); // Chuyển hướng về danh sách thương hiệu sau khi cập nhật thành công
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = new List<string> { $"Lỗi: {ex.Message}" };
                return View(brand);
            }
        }






        // Xóa - AJAX POST
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return Json(new { success = false, message = "Thương hiệu không tồn tại." });
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Brand_ID == id);
        }
    }
}
