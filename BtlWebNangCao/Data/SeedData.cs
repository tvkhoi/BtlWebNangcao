using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace BtlWebNangCao.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                // Kiểm tra nếu đã có admin thì không cần tạo mới
                if (await context.NguoiDungs.AnyAsync(u => u.VaiTro == "Admin"))
                {
                    return;
                }

                // Mã hóa mật khẩu
                var passwordHasher = new PasswordHasher<NguoiDung>();
                var hashedPassword = passwordHasher.HashPassword(null, "Admin@123");

                // Thêm admin mới
                var adminUser = new NguoiDung
                {
                    MaNguoiDung = Guid.NewGuid().ToString(),
                    TenDangNhap = "admin",
                    MatKhau = hashedPassword,
                    Email = "admin@example.com",
                    VaiTro = "Admin",
                    NgayTao = DateTime.Now
                };

                context.NguoiDungs.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }


}
