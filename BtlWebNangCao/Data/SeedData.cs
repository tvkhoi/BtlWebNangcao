using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Kiểm tra nếu đã có vai trò Admin thì không cần tạo mới
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                // Tạo vai trò Admin
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Kiểm tra nếu đã có admin thì không cần tạo mới
            if (await userManager.Users.AnyAsync(u => u.UserName == "admin"))
            {
                return;
            }

            // Tạo người dùng admin mới
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true, // Đánh dấu email đã xác nhận
                // Các thuộc tính khác nếu cần
            };

            // Tạo người dùng với mật khẩu
            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                // Gán vai trò Admin cho người dùng
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}