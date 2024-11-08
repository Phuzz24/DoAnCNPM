using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Controllers
{
    public class BrandController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public BrandController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // Tìm kiếm
        public async Task<IActionResult> DSBrand(string searchTerm)
        {
            var brands = string.IsNullOrEmpty(searchTerm) ? _context.Brands :
                _context.Brands.Where(b => b.BrandName.Contains(searchTerm));
            return View(await brands.ToListAsync());
        }

        // Thêm mới
        [HttpGet]
        public IActionResult Create() => View();

        // POST: Brand/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DSBrand));
            }
            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand)
        {
            if (id != brand.Brand_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Brand_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DSBrand));
            }
            return View(brand);
        }

        // GET: Brand/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Brand_ID == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DSBrand));
        }
        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Brand_ID == id);
        }
    }
}
