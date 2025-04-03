using BtlWebNangCao.Models;
using Microsoft.AspNetCore.Identity;

namespace BtlWebNangCao.Services
{
    public class ResetActiveUsersService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public ResetActiveUsersService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var nextMidnight = DateTime.Today.AddDays(1).AddMilliseconds(-1); // Thời điểm nửa đêm
            var timeUntilNextMidnight = nextMidnight - DateTime.Now;

            _timer = new Timer(ResetActiveUsers, null, timeUntilNextMidnight, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void ResetActiveUsers(object state)
        {
            // Lấy một scope mới để truy cập vào DbContext
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Lấy tất cả người dùng và reset LastActiveDate
                var users = userManager.Users.Where(u => u.LastActiveDate.HasValue).ToList();
                foreach (var user in users)
                {
                    // Reset LastActiveDate về null hoặc một giá trị mặc định nào đó
                    user.LastActiveDate = null;
                    await userManager.UpdateAsync(user);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
