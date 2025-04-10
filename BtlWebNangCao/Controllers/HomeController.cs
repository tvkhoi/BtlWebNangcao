using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BtlWebNangCao.Controllers
{
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? maPhong)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return Challenge(); // hoặc chuyển hướng đăng nhập
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Challenge();
            }


            var danhSachPhong = await _context.ThanhVienPhongs
                .Where(tv => tv.MaNguoiDung == user.Id)
                .Include(tv => tv.PhongChat)
                .Select(tv => tv.PhongChat)
                .ToListAsync();

            PhongChat? phongDuocChon = null;
            List<TinNhan> tinNhanTrongPhong = new();

            if (maPhong.HasValue)
            {
                phongDuocChon = await _context.PhongChats
                    .Include(p => p.DanhSachTinNhan.OrderBy(t => t.NgayGui))
                    .ThenInclude(t => t.NguoiGui)
                    .FirstOrDefaultAsync(p => p.MaPhong == maPhong.Value);

                if (phongDuocChon != null)
                {
                    tinNhanTrongPhong = phongDuocChon.DanhSachTinNhan.ToList();
                }
            }

            ViewBag.DanhSachPhong = danhSachPhong;
            ViewBag.MaPhongDangChon = maPhong;
            ViewBag.TinNhanTrongPhong = tinNhanTrongPhong;
            ViewBag.TenNguoiDung = user.UserName;

            return View();
        }
    }
}
  