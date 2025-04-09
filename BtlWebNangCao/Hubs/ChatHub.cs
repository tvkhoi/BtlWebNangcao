using BtlWebNangCao.Data;
using BtlWebNangCao.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Hubs
{
	public class ChatHub : Hub
	{
		private readonly ApplicationDbContext _context;

		public ChatHub(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task SendMessage(string user, string message)
		{
			var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user);
			if (userEntity != null)
			{
				var chatMessage = new TinNhan
				{
					MaNguoiGui = userEntity.Id,
					NoiDung = message,
					NgayGui = DateTime.UtcNow
				};

				_context.TinNhans.Add(chatMessage);
				await _context.SaveChangesAsync();

				await Clients.All.SendAsync("ReceiveMessage", user, message);
			}
		}
	}
}
