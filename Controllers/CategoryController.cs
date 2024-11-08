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

        // GET: Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DSCategory));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Category_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Category_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DSCategory));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Category_ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DSCategory));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Category_ID == id);
        }
    }
}
