using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using BtlWebNangCao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Controllers.Admin
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ThanhVienPhongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThanhVienPhongController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index(string searchUser, int? roomId, int page = 1)
        {
            int pageSize = 10;
            var thanhVienData = _context.ThanhVienPhongs
                .Include(tv => tv.PhongChat)
                .Include(tv => tv.NguoiDung)
                .Select(tv => new ThanhVienPhongDTO
                {
                    MaNguoiDung = tv.MaNguoiDung,
                    TenNguoiDung = tv.NguoiDung.UserName,
                    MaPhong = tv.MaPhong,
                    TenPhong = tv.PhongChat.TieuDe,
                    VaiTro = tv.VaiTroPhong,
                    NgayThamGia = tv.NgayThamGia,
                    LaNguoiTao = tv.MaNguoiDung == tv.PhongChat.MaNguoiTao
                });

            if (!string.IsNullOrEmpty(searchUser))
                thanhVienData = thanhVienData.Where(tv => tv.TenNguoiDung.Contains(searchUser));
            if (roomId.HasValue)
                thanhVienData = thanhVienData.Where(tv => tv.MaPhong == roomId.Value);

            var totalItems = await thanhVienData.CountAsync();
            var dataPaged = await thanhVienData.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var model = new ThanhVienPhongViewModel
            {
                DanhSachThanhVien = dataPaged,
                TongTrang = (int)Math.Ceiling(totalItems / (double)pageSize),
                TrangHienTai = page
            };

            ViewBag.SearchUser = searchUser;
            ViewBag.SelectedRoomId = roomId?.ToString();
            ViewBag.Rooms = new SelectList(await _context.PhongChats.ToListAsync(), "MaPhong", "TieuDe");

            return View("~/Views/Admin/ThanhVienPhong/Index.cshtml",model);
        }

        [HttpPost("UpdateVaiTro")]
        public async Task<IActionResult> UpdateVaiTro(string maNguoiDung, int maPhong, string vaiTro)
        {
            var thanhVien = await _context.ThanhVienPhongs.FindAsync(maNguoiDung, maPhong);
            if (thanhVien != null)
            {
                thanhVien.VaiTroPhong = vaiTro;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { roomId = maPhong });
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(string userId, int roomId)
        {
            var thanhVien = await _context.ThanhVienPhongs.FindAsync(userId, roomId);
            if (thanhVien != null)
            {
                _context.ThanhVienPhongs.Remove(thanhVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { roomId });
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new ThanhVienPhongCreateViewModel
            {
                DanhSachNguoiDung = _context.Users
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.UserName })
                    .ToList(),

                DanhSachPhong = _context.PhongChats
                    .Select(p => new SelectListItem { Value = p.MaPhong.ToString(), Text = p.TieuDe })
                    .ToList(),

                VaiTro = "Member"
            };

            return View("~/Views/Admin/ThanhVienPhong/Create.cshtml", model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ThanhVienPhongCreateViewModel model)
        {
            if (string.IsNullOrEmpty(model.MaNguoiDung) || model.MaPhong == 0)
            {
                ViewBag.ErrorMessage = "Vui lòng chọn người dùng và phòng chat.";
                return Create();
            }

            var tonTai = await _context.ThanhVienPhongs
                .AnyAsync(tv => tv.MaNguoiDung == model.MaNguoiDung && tv.MaPhong == model.MaPhong);

            if (tonTai)
            {
                ViewBag.ErrorMessage = "Thành viên này đã có trong phòng.";
                return Create();
            }

            var tvMoi = new ThanhVienPhong
            {
                MaNguoiDung = model.MaNguoiDung,
                MaPhong = model.MaPhong,
                VaiTroPhong = model.VaiTro,
                NgayThamGia = DateTime.Now
            };

            _context.ThanhVienPhongs.Add(tvMoi);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { roomId = model.MaPhong });
        }


    }
}
