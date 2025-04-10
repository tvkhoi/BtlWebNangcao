using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace BtlWebNangCao.Hubs
{
    [Authorize(Roles = "User")]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        // Key: userId, Value: List of connectionIds
        private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                OnlineUsers.AddOrUpdate(userId,
                    new List<string> { Context.ConnectionId },
                    (key, oldList) =>
                    {
                        oldList.Add(Context.ConnectionId);
                        return oldList;
                    });

                await BroadcastOnlineUsers();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId) && OnlineUsers.ContainsKey(userId))
            {
                OnlineUsers[userId].Remove(Context.ConnectionId);
                if (!OnlineUsers[userId].Any())
                {
                    OnlineUsers.TryRemove(userId, out _);
                    await BroadcastOnlineUsers();
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room-{roomId}");
        }

        public async Task SendMessageToRoom(string userName, int roomId, string message)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tinNhan = new TinNhan
            {
                MaPhong = roomId,
                MaNguoiGui = userId,
                NoiDung = message,
                NgayGui = DateTime.Now
            };

            _context.TinNhans.Add(tinNhan);
            await _context.SaveChangesAsync();

            await Clients.Group($"room-{roomId}").SendAsync("ReceiveMessage", new
            {
                maPhong = roomId,
                nguoiGui = userName,
                nguoiGuiId = userId,
                noiDung = message,
                ngayGui = tinNhan.NgayGui.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        private async Task BroadcastOnlineUsers()
        {
            var onlineUserIds = OnlineUsers.Keys.ToList();
            var users = await _context.Users
                .Where(u => onlineUserIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            await Clients.All.SendAsync("UsersOnline", users);
        }
    }
}
