using BtlWebNangCao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Controllers.Api
{
    [Route("api/messages")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class MessagesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/messages/{roomId}
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetMessagesInRoom(int roomId)
        {
            var messages = await _context.TinNhans
                .Where(t => t.MaPhong == roomId)
                .OrderBy(t => t.NgayGui)
                .Select(t => new
                {
                    nguoiGui = t.NguoiGui.UserName,
                    nguoiGuiId = t.MaNguoiGui,
                    noiDung = t.NoiDung,
                    ngayGui = t.NgayGui
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}
