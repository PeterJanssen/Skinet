using Application.Core.Services.Interfaces.Identity.JWT;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Services.Implementations.Identity.JWT
{
    public class JwtRefreshTokenCache : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IJwtAuthManager _jwtAuthManager;

        public JwtRefreshTokenCache(IJwtAuthManager jwtAuthManager)
        {
            _jwtAuthManager = jwtAuthManager;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _jwtAuthManager.RemoveExpiredRefreshTokens(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
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
