using Barangay.Services;

namespace Barangay.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Run every hour

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token cleanup service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                        var cleanedCount = await tokenService.CleanupExpiredTokensAsync();
                        
                        if (cleanedCount > 0)
                        {
                            _logger.LogInformation("Cleaned up {Count} expired tokens", cleanedCount);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during token cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Token cleanup service stopped");
        }
    }
}
