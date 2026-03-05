using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.BackgroundServices
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SessionCleanupService> _logger;

        public SessionCleanupService(IServiceProvider services, ILogger<SessionCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Session Cleanup Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

                        // Expire sessions inactive for more than 2 hours
                        await sessionService.ExpireStaleSessionsAsync(timeoutMinutes: 120);
                    }

                    // Run cleanup every 30 minutes
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Session Cleanup Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            _logger.LogInformation("Session Cleanup Service stopped.");
        }
    }
}