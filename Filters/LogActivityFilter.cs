using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace DoAnCNPM.Filters
{
    public class LogActivityFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Thực hiện trước khi một Action trong Controller được gọi
            var httpContext = context.HttpContext;

            // Lấy thông tin về Controller và Action hiện tại
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Lưu thông tin hoạt động
            var activity = $"Admin đã thực hiện: {controller}/{action} lúc {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            // Lấy danh sách các hoạt động từ Session
            var activities = httpContext.Session.GetObjectFromJson<List<string>>("AdminActivities") ?? new List<string>();

            // Thêm hoạt động mới
            activities.Add(activity);

            // Cập nhật lại Session
            httpContext.Session.SetObjectAsJson("AdminActivities", activities);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Không cần thực hiện gì sau khi Action kết thúc
        }
    }
}
