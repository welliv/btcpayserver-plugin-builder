using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BTCPayServer.Plugins.Shopstr.Services;

public class ShopstrBackgroundService : BackgroundService
{
    private readonly ILogger<ShopstrBackgroundService> _logger;

    public ShopstrBackgroundService(ILogger<ShopstrBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Shopstr background synchronization loop started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Future implementation: poll for queued listing publications and DM fetches
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error inside Shopstr synchronization loop.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("Shopstr background synchronization loop stopped.");
    }
}
