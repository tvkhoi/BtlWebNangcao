using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BtlWebNangCao.Controllers.Api
{
    [Route("api/rooms")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class RoomsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoomsApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/rooms/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRooms()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rooms = await _context.ThanhVienPhongs
                .Where(t => t.MaNguoiDung == userId)
                .Include(t => t.PhongChat)
                .Select(t => new
                {
                    t.PhongChat.MaPhong,
                    t.PhongChat.TieuDe,
                    t.PhongChat.LaCongKhai
                })
                .ToListAsync();

            return Ok(rooms);
        }
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicRooms()
        {
            var rooms = await _context.PhongChats
                .Where(p => p.LaCongKhai)
                .Select(p => new
                {
                    maPhong = p.MaPhong,
                    tieuDe = p.TieuDe
                })
                .ToListAsync();

            return Ok(rooms);
        }

        // POST: api/rooms/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var room = new PhongChat
            {
                TieuDe = model.TieuDe,
                MoTa = model.MoTa,
                LaCongKhai = model.LaCongKhai,
                MaNguoiTao = userId,
                NgayTao = DateTime.Now
            };

            _context.PhongChats.Add(room);
            await _context.SaveChangesAsync();

            // Tự động thêm người tạo làm thành viên
            _context.ThanhVienPhongs.Add(new ThanhVienPhong
            {
                MaNguoiDung = userId,
                MaPhong = room.MaPhong,
                VaiTroPhong = "Moderator"
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                roomId = room.MaPhong,
                roomName = room.TieuDe
            });

        }
    }

    public class CreateRoomRequest
    {
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public bool LaCongKhai { get; set; }
    }
}
