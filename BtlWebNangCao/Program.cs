using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using BtlWebNangCao.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Hỗ trợ Razor Pages (cho Identity UI)

// đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  // Yêu cầu xác nhận tài khoản qua email
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ "; // phạm vi giá trị mã tên tài khoản có thẻ nhận
    options.User.RequireUniqueEmail = true; // Email phải là duy nhất
    options.Password.RequireDigit = true;           // Bắt buộc có số
    options.Password.RequireLowercase = true;       // Bắt buộc có chữ thường
    options.Password.RequireUppercase = true;       // Bắt buộc có chữ hoa
    options.Password.RequireNonAlphanumeric = true; // Bắt buộc có ký tự đặc biệt
    options.Password.RequiredLength = 6;            // Độ dài tối thiểu là 6
    //options.Lockout.MaxFailedAccessAttempts = 5;    // Khóa tài khoản sau 5 lần nhập sai
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // Khóa trong 10 phút
}).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Configuration.AddUserSecrets<Program>(); // Đọc từ User Secrets

// Đọc cấu hình SmtpSettings từ appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
// Đăng ký dịch vụ SmtpEmailSender để sử dụng cho IEmailSender
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14); // Giữ đăng nhập trong 14 ngày
    options.SlidingExpiration = true; // Gia hạn cookie mỗi khi người dùng hoạt động
    options.LoginPath = "/Identity/Account/Login"; // Đường dẫn trang đăng nhập
    options.LogoutPath = "/Identity/Account/Logout"; // Đường dẫn trang đăng xuất
});

// Thêm dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cần thiết cho việc chạy ứng dụng
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession(); // Bật middleware session

app.UseAuthentication(); // Bật middleware xác thực
app.UseAuthorization();

// Middleware kiểm tra trạng thái đăng nhập và lần đầu truy cập
app.Use(async (context, next) =>
{
    // Kiểm tra xem người dùng đã đăng nhập chưa
    if (context.User.Identity.IsAuthenticated)
    {
        // Nếu đã đăng nhập, cho phép tiếp tục truy cập
        await next();
    }
    else if (context.Session.GetString("IsFirstVisit") == null)
    {
        // Nếu là lần đầu truy cập
        context.Session.SetString("IsFirstVisit", "false");

        // Điều hướng đến trang đăng ký
        context.Response.Redirect("/Identity/Account/Login");
    }
    else
    {
        // Cho phép tiếp tục truy cập nếu không phải lần đầu truy cập và chưa đăng nhập
        await next();
    }
});


app.MapRazorPages(); // Đăng ký Razor Pages để sử dụng Identity UI

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
