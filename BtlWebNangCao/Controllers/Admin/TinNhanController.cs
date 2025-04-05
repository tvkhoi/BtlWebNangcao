using BtlWebNangCao.Data;
using BtlWebNangCao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BtlWebNangCao.Controllers.Admin
{
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class TinNhanController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TinNhanController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// Trang quản lý tin nhắn theo phòng
        public async Task<IActionResult> Index(int? phongChatId, int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            // Lấy danh sách phòng chat để hiển thị dropdown
            ViewBag.PhongChats = _context.PhongChats
                .Where(p => p.LaCongKhai == true)  // Lọc phòng công khai
                .Select(p => new PhongChatDropDownViewModel
                {
                MaPhong = p.MaPhong,
                sTieuDe = p.TieuDe
                }).ToList();


            // Truy vấn danh sách tin nhắn theo phòng chat
            var messages = _context.TinNhans
                .Include(m => m.NguoiGui)
                .Include(m => m.PhongChat)
                .Where(m => !phongChatId.HasValue || m.MaPhong == phongChatId) // Lọc theo phòng chat nếu có
                .OrderByDescending(m => m.NgayGui)
                .Select(m => new TinNhanViewModel
                {
                    MaTinNhan = m.MaTinNhan,
                    NoiDung = m.NoiDung,
                    NguoiGui = m.NguoiGui.UserName,
                    NgayGui = m.NgayGui,
                    PhongChat = m.PhongChat.TieuDe
                });

            return View("~/Views/Admin/TinNhan/Index.cshtml", await messages.ToPagedListAsync(pageNumber, pageSize));
        }

        // Xóa tin nhắn
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.TinNhans.FindAsync(id);
            if (message == null) return Json(new { success = false, message = "Tin nhắn không tìm thấy" });

            _context.TinNhans.Remove(message);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}
