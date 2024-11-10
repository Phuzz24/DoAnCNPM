using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public CategoryController(DBDienThoaiContext context)
        {
            _context = context;
        }
        // GET: Category
        public async Task<IActionResult> DSCategory(string searchTerm)
        {
            var categories = from c in _context.Categories
                             select c;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = categories.Where(c => c.CategoryName.Contains(searchTerm));
            }

            return View(await categories.ToListAsync());
        }

        // Thêm mới - GET
        [HttpGet]
        public IActionResult Create() => View();

        // Thêm mới - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm loại thành công!";
                return RedirectToAction(nameof(DSCategory));
            }
            return View(category);
        }

        // GET: Edit Category
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm Category dựa trên ID
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "ID loại không hợp lệ.";
                return RedirectToAction("DSCategory");
            }
            return View(category);
        }

        // POST: Edit Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Category_ID)
            {
                TempData["ErrorMessage"] = "ID loại không hợp lệ.";
                return RedirectToAction("DSCategory");
            }

            if (!ModelState.IsValid)
            {
                return View(category); // Trả lại form với thông báo lỗi nếu model không hợp lệ
            }

            try
            {
                // Cập nhật thông tin của Category
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật loại thành công!";
                return RedirectToAction("DSCategory");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.Category_ID == id))
                {
                    TempData["ErrorMessage"] = "Loại không tồn tại.";
                    return RedirectToAction("DSCategory");
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: Delete Category
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Loại không tồn tại." });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Category_ID == id);
        }
    }
}
