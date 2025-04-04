using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using BtlWebNangCao.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Hỗ trợ Razor Pages (cho Identity UI)
// Thêm Logging vào ứng dụng
builder.Logging.ClearProviders(); // Xóa các provider mặc định
builder.Logging.AddConsole(); // Thêm logging ra console
builder.Logging.AddDebug();   // Thêm logging vào Debug Output (cho Visual Studio)

// đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true; // Chỉ truy cập qua HTTP, không thể truy cập từ JavaScript
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Luôn gửi qua HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Bảo mật chống tấn công CSRF
});


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
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Configuration.AddUserSecrets<Program>(); // Đọc từ User Secrets

// Đọc cấu hình SmtpSettings từ appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddScoped<RoleInitializer>();

builder.Services.AddHostedService<ResetActiveUsersService>();

// Đăng ký dịch vụ SmtpEmailSender để sử dụng cho IEmailSender
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

// Thêm dịch vụ nguoidung
//builder.Services.AddScoped<NguoiDungService>();
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Bắt buộc HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Cho phép cookie được gửi qua các yêu cầu cross-site
    options.ExpireTimeSpan = TimeSpan.FromDays(14); // Giữ đăng nhập trong 14 ngày
    options.SlidingExpiration = true; // Gia hạn cookie mỗi khi người dùng hoạt động
    options.LoginPath = "/Identity/Account/Login"; // Đường dẫn trang đăng nhập
    options.LogoutPath = "/Identity/Account/Logout"; // Đường dẫn trang đăng xuất
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Thêm dịch vụ Session
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cần thiết cho việc chạy ứng dụng
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None; // Ngăn chặn tấn công CSRF
    options.Cookie.Name = ".MySampleMVCWeb.Session";
});



var app = builder.Build();

// Gọi SeedData khi ứng dụng khởi động
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedData.Initialize(serviceProvider);

    // Thêm dữ liệu vào AspNetRole trong trường hợp không tồn tại
    var roleInitializer = serviceProvider.GetRequiredService<RoleInitializer>();
    await roleInitializer.InitializeAsync();
}


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

// Middleware kiểm tra trạng thái đăng nhập

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        // Sử dụng email hoặc tên người dùng thay vì userId
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var userName = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser user = null;

        // Tìm người dùng theo email hoặc tên
        if (!string.IsNullOrEmpty(userEmail))
        {
            user = await userManager.FindByEmailAsync(userEmail);
        }
        else if (!string.IsNullOrEmpty(userName))
        {
            user = await userManager.FindByNameAsync(userName);
        }

        if (string.IsNullOrEmpty(role))
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }

        if (role == "Admin")
        {
            // Nếu người dùng đã ở trang Admin, không điều hướng lại
            if (!context.Request.Path.StartsWithSegments("/Admin"))
            {
                context.Response.Redirect("/Admin");
                return; // Dừng xử lý tiếp theo
            }
        }
        else if (role == "User")
        {
            // Nếu người dùng đã ở trang User, không điều hướng lại
            if (!context.Request.Path.StartsWithSegments("/Home"))
            {
                // Cập nhật LastActiveDate khi người dùng thực hiện một hành động
                user.LastActiveDate = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
                context.Response.Redirect("/Home");
                return; // Dừng xử lý tiếp theo
            }
        }
    }
    else
    {
        // Nếu người dùng chưa đăng nhập, cho phép truy cập đến trang đăng nhập, đăng ký và quên mật khẩu
        var path = context.Request.Path.ToString();
        if (!path.StartsWith("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase) &&
            !path.StartsWith("/Identity/Account/ForgotPassword", StringComparison.OrdinalIgnoreCase)&&
            !path.StartsWith("/Identity/Account/ConfirmEmail", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/Identity/Account/Login");
            return; // Dừng xử lý tiếp theo
        }
    }
    await next();
});



app.MapRazorPages(); // Đăng ký Razor Pages để sử dụng Identity UI

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
