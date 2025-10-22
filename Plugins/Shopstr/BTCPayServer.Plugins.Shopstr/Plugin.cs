using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Plugins.Shopstr.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.Shopstr;

public class Plugin : BaseBTCPayServerPlugin
{
    public override string Identifier => "BTCPayServer.Plugins.Shopstr";
    public override string Name => "Shopstr";
    public override string Description => "Publish BTCPay listings to Shopstr and handle Nostr messaging.";

    public override void Execute(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ShopstrSettingsRepository>();
        services.AddSingleton<ShopstrRelayCoordinator>();
        services.AddSingleton<ShopstrLinkGenerator>();
        services.AddHostedService<ShopstrBackgroundService>();
        services.AddSingleton<IUIExtension>(new UIExtension("~/Views/Store/Shopstr/StoreNavShopstr.cshtml", "store-nav"));

        base.Execute(services);
    }
}
