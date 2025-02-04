using DoAnCNPM.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DoAnCNPM.Helpers
{
    public static class ActivityLogger
    {
        public static void LogActivity(HttpContext httpContext, DBDienThoaiContext context, string activity)
        {
            var adminName = httpContext.User.Identity.Name; // Lấy tên admin
            var adminActivity = new AdminActivity
            {
                Activity = activity,
                AdminName = adminName ?? "Unknown",
                Timestamp = DateTime.Now
            };

            context.AdminActivities.Add(adminActivity);
            context.SaveChanges();
        }
    }

}
