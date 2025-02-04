using DoAnCNPM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Http.Features;
using DoAnCNPM.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:44315");

// Cấu hình DbContext
builder.Services.AddDbContext<DBDienThoaiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBDienThoaiConnection")));
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Đường dẫn tới trang đăng nhập
    options.AccessDeniedPath = "/Account/AccessDenied";  // Đường dẫn tới trang từ chối truy cập
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google"; // Đảm bảo khớp với cấu hình trong Google Console
});
//.AddFacebook(facebookOptions =>
//{
//    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
//    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
//    facebookOptions.CallbackPath = "/signin-facebook";
//});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireEmployeeRole", policy => policy.RequireRole("Nhân viên"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var cultureInfo = new CultureInfo("en-US"); // Sử dụng "en-US" để định dạng số với dấu chấm
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddHttpClient();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // Giới hạn 10 MB
});

// Cấu hình các dịch vụ khác
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<LogActivityFilter>(); // Đăng ký LogActivityFilter

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();  // Phải đặt trước UseAuthorization
app.UseAuthorization();

app.UseSession();

// Cấu hình các endpoint
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=TrangChu}/{id?}"
);

app.Run();
