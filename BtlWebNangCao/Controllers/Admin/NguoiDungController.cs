using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using BtlWebNangCao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NguoiDungController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                // Lấy vai trò của người dùng
                var roles = await _userManager.GetRolesAsync(user);

                // Kiểm tra nếu người dùng có vai trò là "User" (không phải Admin)
                if (roles.Contains("User"))
                {
                    userList.Add(new UserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = string.Join(", ", roles),
                        IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
                    });
                }
            }

            return View("~/Views/Admin/NguoiDung/Index.cshtml", userList);
        }

        [HttpPost("Lock/{id}")]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Khóa tài khoản trong 1 năm
            user.LockoutEnd = DateTime.UtcNow.AddYears(1);
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        [HttpPost("Unlock/{id}")]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Mở khóa tài khoản
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }
        [HttpGet("SearchUsers")]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            // Chỉ áp dụng lọc nếu có thuật ngữ tìm kiếm, nếu không sẽ lấy tất cả người dùng
            foreach (var user in users)
            {
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    // Kiểm tra vai trò "Người dùng" và thuật ngữ tìm kiếm
                    if (roles != null && roles.Contains("User"))
                    {
                        bool matchesSearchTerm = string.IsNullOrEmpty(searchTerm) ||
                                                 user.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                 user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

                        if (matchesSearchTerm)
                        {
                            userList.Add(new UserViewModel
                            {
                                Id = user.Id,
                                UserName = user.UserName,
                                Email = user.Email,
                                Roles = string.Join(", ", roles),
                                IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
                            });
                        }
                    }
                }
            }

            return Json(userList);
        }





    }
}
