using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Controllers.Api
{
    [Route("api/members")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class MembersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembersApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUserToRoom([FromBody] AddMemberRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.UserIdentifier))
                return BadRequest("Thiếu thông tin người dùng hoặc phòng.");

            // Tìm người dùng theo ID hoặc Username
            var user = await _userManager.FindByIdAsync(model.UserIdentifier);
            if (user == null)
                user = await _userManager.FindByNameAsync(model.UserIdentifier);

            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            // Kiểm tra tồn tại phòng
            var roomExists = await _context.PhongChats.AnyAsync(p => p.MaPhong == model.RoomId);
            if (!roomExists)
                return NotFound("Không tìm thấy phòng chat.");

            // Kiểm tra đã là thành viên chưa
            var existed = await _context.ThanhVienPhongs
                .AnyAsync(x => x.MaNguoiDung == user.Id && x.MaPhong == model.RoomId);

            if (existed)
                return BadRequest("Người dùng đã là thành viên của phòng.");

            // Thêm mới thành viên
            var newMember = new ThanhVienPhong
            {
                MaNguoiDung = user.Id,
                MaPhong = model.RoomId,
                VaiTroPhong = "Member"
            };

            _context.ThanhVienPhongs.Add(newMember);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm thành viên thành công." });
        }


        public class AddMemberRequest
        {
            public int RoomId { get; set; }
            public string UserIdentifier { get; set; } // có thể là ID hoặc Username
        }
    }
}
