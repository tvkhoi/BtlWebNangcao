using BtlWebNangCao.Data;
using BtlWebNangCao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BtlWebNangCao.Controllers
{
    [Authorize (Roles ="User")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị thông tin cá nhân
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy userId từ claims
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new ProfileViewModel
                {
                    UserName = u.UserName,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Chỉnh sửa thông tin cá nhân
        public IActionResult Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy userId từ claims
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new ProfileViewModel
                {
                    UserName = u.UserName,
                    Email = u.Email
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Cập nhật thông tin cá nhân
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy userId từ claims
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin người dùng
                user.UserName = profileViewModel.UserName;
                user.Email = profileViewModel.Email;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(profileViewModel);
        }

    }
}
