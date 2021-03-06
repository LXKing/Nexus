﻿namespace Aiursoft.Status.Services
{
    using global::Aiursoft.Pylon.Interfaces;
    using global::Aiursoft.Status.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    namespace Aiursoft.Probe.Services
    {
        public class TimedCleaner : IHostedService, IDisposable, ISingletonDependency
        {
            private Timer _timer;
            private readonly ILogger _logger;
            private readonly IServiceScopeFactory _scopeFactory;

            public TimedCleaner(
                ILogger<TimedCleaner> logger,
                IServiceScopeFactory scopeFactory)
            {
                _logger = logger;
                _scopeFactory = scopeFactory;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                _logger.LogInformation("TimedCleaner Background Service is starting.");
                _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(60));
                return Task.CompletedTask;
            }

            private async void DoWork(object state)
            {
                try
                {
                    _logger.LogInformation("Cleaner task started!");
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<StatusDbContext>();
                        await AllClean(dbContext);
                    }
                    _logger.LogInformation("Cleaner task finished!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cleaner crashed!");
                }
            }

            private async Task AllClean(StatusDbContext dbContext)
            {
                var oldestRecordTime = DateTime.UtcNow - TimeSpan.FromDays(7);
                var items = await dbContext.ErrorLogs.Where(t => t.LogTime < oldestRecordTime).ToListAsync();
                dbContext.ErrorLogs.RemoveRange(items);
                await dbContext.SaveChangesAsync();
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _logger.LogInformation("Timed Background Service is stopping.");
                _timer?.Change(Timeout.Infinite, 0);
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                _timer?.Dispose();
            }
        }
    }

}
