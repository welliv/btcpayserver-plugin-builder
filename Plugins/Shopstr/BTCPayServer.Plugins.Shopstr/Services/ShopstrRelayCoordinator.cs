using BTCPayServer.Plugins.Shopstr.Models;
using Microsoft.Extensions.Logging;

namespace BTCPayServer.Plugins.Shopstr.Services;

public class ShopstrRelayCoordinator
{
    private readonly ILogger<ShopstrRelayCoordinator> _logger;

    public ShopstrRelayCoordinator(ILogger<ShopstrRelayCoordinator> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<string> NormalizeRelays(ShopstrSettings settings)
    {
        var relays = settings.GetRelayUris();
        if (relays.Count == 0)
        {
            _logger.LogInformation("Shopstr plugin using default relay set for store with missing configuration.");
        }

        return relays;
    }
}
