# Shopstr BTCPay Server Plugin scaffold

This directory contains the starter plugin that was generated to integrate [Shopstr](https://github.com/shopstr-eng/shopstr) with BTCPay Server. The current code only persists per-store Nostr credentials and exposes a settings page; no Nostr publishing or DM logic has been implemented yet. The guide below explains how the scaffold is wired together and highlights the main seams you can extend. Each step references source files so you can open them quickly or copy/paste examples.

## 1. Register plugin services

The entry point lives in [`Plugin.cs`](BTCPayServer.Plugins.Shopstr/Plugin.cs). Service registrations include:

- `ShopstrSettingsRepository` for persisting per-store configuration.
- `ShopstrRelayCoordinator` for normalizing relay input.
- `ShopstrLinkGenerator` for building the settings URL.
- `ShopstrBackgroundService` placeholder for future sync jobs.
- A `store-nav` UI extension that adds the Shopstr tab to the BTCPay store navigation.

> Copy/paste reference:
>
> ```csharp
> services.AddSingleton<ShopstrSettingsRepository>();
> services.AddSingleton<ShopstrRelayCoordinator>();
> services.AddSingleton<ShopstrLinkGenerator>();
> services.AddHostedService<ShopstrBackgroundService>();
> services.AddSingleton<IUIExtension>(new UIExtension(
>     "~/Views/Store/Shopstr/StoreNavShopstr.cshtml",
>     "store-nav"));
> ```
>
> — [`Plugin.cs`](BTCPayServer.Plugins.Shopstr/Plugin.cs)

## 2. Understand the store settings model

[`Models/ShopstrSettings.cs`](BTCPayServer.Plugins.Shopstr/Models/ShopstrSettings.cs) defines the data captured on the settings page: merchant Nostr keys, relay list, and feature toggles. The `GetRelayUris` helper already trims and deduplicates comma-separated values—reuse it when connecting to relays.

> Copy/paste reference:
>
> ```csharp
> public IReadOnlyList<string> GetRelayUris()
> {
>     if (string.IsNullOrWhiteSpace(RelayList))
>     {
>         return Array.Empty<string>();
>     }
>
>     return RelayList
>         .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
>         .Distinct(StringComparer.OrdinalIgnoreCase)
>         .ToArray();
> }
> ```
>
> — [`ShopstrSettings.cs`](BTCPayServer.Plugins.Shopstr/Models/ShopstrSettings.cs)

## 3. Persist settings with `ShopstrSettingsRepository`

[`Services/ShopstrSettingsRepository.cs`](BTCPayServer.Plugins.Shopstr/Services/ShopstrSettingsRepository.cs) wraps BTCPay's `ISettingsRepository`. Use `SetStoreSettings` whenever the UI posts updates, and call `GetStoreSettings` to hydrate the view or background jobs. The settings are stored under the namespaced key `BTCPayServer.Plugins.Shopstr.Settings`.

```csharp
await _settingsRepository.SetStoreSettings(storeId, settings);
var settings = await _settingsRepository.GetStoreSettings(storeId);
```

## 4. Wire up the Razor UI

The configuration screen is implemented in [`Views/Store/Shopstr/Index.cshtml`](BTCPayServer.Plugins.Shopstr/Views/Store/Shopstr/Index.cshtml). The page posts to `ShopstrController.Update`, which then persists settings. The nav tab lives in [`StoreNavShopstr.cshtml`](BTCPayServer.Plugins.Shopstr/Views/Store/Shopstr/StoreNavShopstr.cshtml) and uses `ShopstrLinkGenerator` to create the URL.

Key snippets you can reuse:

- Posting the form:

  ```html
  <form asp-action="Update" asp-route-storeId="@storeId" method="post" class="card">
      ...
      <button type="submit" class="btn btn-primary">Save settings</button>
  </form>
  ```

- Store navigation link:

  ```csharp
  var href = LinkGenerator.GetStoreDashboardUrl(storeId);
  ```

## 5. Controller endpoints

[`Controllers/ShopstrController.cs`](BTCPayServer.Plugins.Shopstr/Controllers/ShopstrController.cs) exposes two actions:

- `Index` loads settings, prepares the relay preview, and renders the Razor page.
- `Update` validates the posted model, saves settings, and redirects back to `Index` with a flash message.

Use these actions as the place to invoke additional validation (e.g., Nostr key format checks) or to enqueue background jobs after settings change.

## 6. Background workflow placeholder

[`Services/ShopstrBackgroundService.cs`](BTCPayServer.Plugins.Shopstr/Services/ShopstrBackgroundService.cs) currently logs a heartbeat every five minutes. Replace the TODO block with logic that publishes queued listings and polls for encrypted DMs. The structure is already set up with cancellation handling and retry logging.

```csharp
while (!stoppingToken.IsCancellationRequested)
{
    try
    {
        // TODO: publish listings, fetch DMs
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error inside Shopstr synchronization loop.");
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}
```

## 7. Solution integration

The plugin project is already part of `btcpayserver-plugin-builder.sln`, so opening the solution in Visual Studio or Rider will load it automatically. The `.csproj` targets .NET 8 and references BTCPay abstractions and client libraries. Adjust package versions to match the BTCPay Server release you build against.

## Next implementation steps

1. Pick a Nostr client library for .NET and register it in `Plugin.Execute`.
2. Extend the background service to push product updates to relays.
3. Listen for encrypted DMs (NIP-04), persist them, and surface conversations in the UI.
4. Integrate with a hosted Shopstr marketplace dashboard if you plan to offer it as a managed service.

Following the structure above ensures new logic slots neatly into the scaffold without rewriting the existing plumbing.
