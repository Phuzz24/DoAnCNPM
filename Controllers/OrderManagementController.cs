using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoAnCNPM.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly DBDienThoaiContext _context;

        public OrderManagementController(DBDienThoaiContext context)
        {
            _context = context;
        }

        // GET: Order/Index
        public async Task<IActionResult> DSDonHang(string searchTerm)
        {
            var orders = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o => o.CustomerName.Contains(searchTerm) ||
                                           o.Status.Contains(searchTerm) ||
                                           o.PaymentMethod.Contains(searchTerm));
            }

            return View(await orders.Include(o => o.Customer).ToListAsync());
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order created successfully!";
                return RedirectToAction(nameof(DSDonHang));
            }
            return View(order);
        }

        // GET: Order/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Order_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Order updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Orders.Any(e => e.Order_ID == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(DSDonHang));
            }
            return View(order);
        }

        // GET: Order/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Order deleted successfully!";
            return RedirectToAction(nameof(DSDonHang));
        }
    }
}
