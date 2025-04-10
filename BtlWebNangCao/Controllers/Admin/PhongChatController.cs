using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using BtlWebNangCao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BtlWebNangCao.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PhongChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PhongChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string search)
        {
            var phongChatsQuery = _context.PhongChats
                .Include(p => p.NguoiTao)
                .Select(p => new PhongChatViewModel
                {
                    Pk_iMaPhong = p.MaPhong,
                    sTieuDe = p.TieuDe,
                    sMoTa = p.MoTa,
                    bLaCongKhai = p.LaCongKhai,
                    dNgayTao = p.NgayTao,
                    TenNguoiTao = p.NguoiTao.UserName
                });

            if (!string.IsNullOrEmpty(search))
            {
                phongChatsQuery = phongChatsQuery.Where(p => p.sTieuDe.Contains(search));
            }

            var phongChats = await phongChatsQuery.ToListAsync();
            return View("~/Views/Admin/PhongChat/Index.cshtml", phongChats);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/PhongChat/Create.cshtml");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(PhongChatViewModel model)
        {
            // Lấy userId từ claims
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            // Gỡ lỗi vì TenNguoiTao không nhập từ form
            ModelState.Remove(nameof(model.TenNguoiTao));
            if (ModelState.IsValid)
            {
                var phong = new PhongChat
                {
                    TieuDe = model.sTieuDe,
                    MoTa = model.sMoTa,
                    LaCongKhai = model.bLaCongKhai,
                    NgayTao = DateTime.Now,
                    MaNguoiTao = user.Id
                };

                _context.PhongChats.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View("~/Views/Admin/PhongChat/Create.cshtml", model);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var phong = await _context.PhongChats.FindAsync(id);
            if (phong == null)
                return NotFound();

            var model = new PhongChatViewModel
            {
                Pk_iMaPhong = phong.MaPhong,
                sTieuDe = phong.TieuDe,
                sMoTa = phong.MoTa,
                bLaCongKhai = phong.LaCongKhai
            };

            return View("~/Views/Admin/PhongChat/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, PhongChatViewModel model)
        {
            // Gỡ lỗi vì TenNguoiTao không nhập từ form
            ModelState.Remove(nameof(model.TenNguoiTao));
            var phong = await _context.PhongChats.FindAsync(id);
            if (phong == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                phong.TieuDe = model.sTieuDe;
                phong.MoTa = model.sMoTa;
                phong.LaCongKhai = model.bLaCongKhai;

                _context.Update(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View("~/Views/Admin/PhongChat/Edit.cshtml", model);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var phong = await _context.PhongChats.FindAsync(id);
            if (phong == null)
                return NotFound();

            // Xóa thành viên trong phòng
            var members = _context.ThanhVienPhongs.Where(tv => tv.MaPhong == id);
            _context.ThanhVienPhongs.RemoveRange(members);

            // Xóa tin nhắn trong phòng
            var messages = _context.TinNhans.Where(tn => tn.MaPhong == id);
            _context.TinNhans.RemoveRange(messages);

            // Xóa phòng chat
            _context.PhongChats.Remove(phong);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Quay lại trang Index
            return RedirectToAction("Index");
        }


        [HttpGet("SearchRooms")]
        public async Task<IActionResult> SearchRooms(string searchTerm)
        {
            var phongChats = await _context.PhongChats
                .Include(p => p.NguoiTao)
                .Where(p => string.IsNullOrEmpty(searchTerm) || p.TieuDe.Contains(searchTerm))
                .Select(p => new PhongChatViewModel
                {
                    Pk_iMaPhong = p.MaPhong,
                    sTieuDe = p.TieuDe,
                    sMoTa = p.MoTa,
                    bLaCongKhai = p.LaCongKhai,
                    dNgayTao = p.NgayTao,
                    TenNguoiTao = p.NguoiTao.UserName
                })
                .ToListAsync();

            return Json(phongChats);
        }
    }
}
