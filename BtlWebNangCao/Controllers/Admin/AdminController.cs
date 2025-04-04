using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using BtlWebNangCao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var totalUsers = users.Count(u => !(_userManager.IsInRoleAsync(u, "Admin").Result));

            var totalRooms = _context.PhongChats.Count();
            var totalMessages = _context.TinNhans.Count();
            var activeUsersToday = _userManager.Users.Count(u => u.LastActiveDate.HasValue && u.LastActiveDate.Value.Date == DateTime.Today);

            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            // Lấy tin nhắn trong 30 ngày gần nhất
            var recentMessages = await _context.TinNhans
                .Where(t => t.NgayGui >= thirtyDaysAgo)
                .GroupBy(t => t.NgayGui.Date)  // Nhóm theo ngày gửi
                .Select(group => new
                {
                    Date = group.Key,
                    Count = group.Count()
                })
                .OrderBy(t => t.Date)  // Sắp xếp theo ngày
                .ToListAsync();

            // Chuẩn bị dữ liệu cho biểu đồ
            var labels = recentMessages.Select(r => r.Date.ToString("dd/MM")).ToList();  // Dữ liệu nhãn (ngày)
            var data = recentMessages.Select(r => r.Count).ToList();  // Dữ liệu số lượng tin nhắn mỗi ngày

            // Lấy danh sách phòng chat (Công khai và Riêng tư)
            var roomTypes = new List<int>
            {
                await _context.PhongChats.CountAsync(p => p.LaCongKhai), // Số phòng công khai
                await _context.PhongChats.CountAsync(p => !p.LaCongKhai) // Số phòng riêng tư
            };

            // Tạo ViewModel và truyền dữ liệu
            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalRooms = totalRooms,
                TotalMessages = totalMessages,
                ActiveUsersToday = activeUsersToday,
                RoomTypes = roomTypes,
                TotalMessagesPerDayLabels = labels,
                TotalMessagesPerDay = data
            };

            return View(model);
        }


    }
}
