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
    //options.Password.RequireDigit = true;           // Bắt buộc có số
    //options.Password.RequireLowercase = true;       // Bắt buộc có chữ thường
    //options.Password.RequireUppercase = true;       // Bắt buộc có chữ hoa
    //options.Password.RequireNonAlphanumeric = true; // Bắt buộc có ký tự đặc biệt
    //options.Password.RequiredLength = 8;            // Độ dài tối thiểu là 8
    //options.Lockout.MaxFailedAccessAttempts = 5;    // Khóa tài khoản sau 5 lần nhập sai
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // Khóa trong 10 phút
}).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Configuration.AddUserSecrets<Program>(); // Đọc từ User Secrets

// Đọc cấu hình SmtpSettings từ appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
// Đăng ký dịch vụ SmtpEmailSender để sử dụng cho IEmailSender
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();


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


app.UseAuthentication(); // Bật middleware xác thực
app.UseAuthorization();

app.MapRazorPages(); // Đăng ký Razor Pages để sử dụng Identity UI

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
