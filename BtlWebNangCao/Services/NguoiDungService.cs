using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Services
{
    [Authorize(Roles = "Admin")] // Bắt buộc chỉ Admin mới vào được
    public class NguoiDungService
    {
        private readonly ApplicationDbContext _contextDb;
        private readonly UserManager<ApplicationUser> _userManager;

        public NguoiDungService(ApplicationDbContext contextDb, UserManager<ApplicationUser> userManager)
        {
            _contextDb = contextDb;
            _userManager = userManager;
        }

        // Lấy vai trò của người dùng theo email
        public async Task<string> GetUserRoleAsync(string email)
        {
            var user = await _contextDb.NguoiDungs.FirstOrDefaultAsync(x => x.Email == email);
            return user?.VaiTro ?? "User";  // Mặc định trả về "User" nếu không tìm thấy
        }

        // Cập nhật vai trò người dùng
        public async Task<bool> UpdateUserRoleAsync(string email, string newRole)
        {
            var user = await _contextDb.NguoiDungs.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                user.VaiTro = newRole;
                _contextDb.NguoiDungs.Update(user);
                await _contextDb.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Lấy danh sách người dùng theo vai trò
        public async Task<List<NguoiDung>> GetUsersByRoleAsync(string role)
        {
            return await _contextDb.NguoiDungs.Where(u => u.VaiTro == role).ToListAsync();
        }
    }
}
