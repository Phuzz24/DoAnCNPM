using DoAnCNPM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Controllers
{
    public class NotificationController : Controller
    {
        private readonly DBDienThoaiContext context;
        public NotificationController(DBDienThoaiContext _context)
        {
            context = _context;
        }

        //[HttpGet]
        //public IActionResult GetNotifications()
        //{
        //    var adminName = User.Identity.Name;
        //    var notifications = context.Admins;
        //        .Where(log => log.AdminName == adminName)
        //        .OrderByDescending(log => log.Timestamp)
        //        .Take(20) // Giới hạn số thông báo trả về
        //        .ToList();

        //    return Json(notifications);
        //}
    }
}
