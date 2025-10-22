using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.Shopstr.Models;

namespace BTCPayServer.Plugins.Shopstr.Services;

public class ShopstrSettingsRepository
{
    private const string SettingsKey = "BTCPayServer.Plugins.Shopstr.Settings";
    private readonly ISettingsRepository _settingsRepository;

    public ShopstrSettingsRepository(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<ShopstrSettings> GetStoreSettings(string storeId)
    {
        var settings = await _settingsRepository.GetSettingAsync<ShopstrSettings>(SettingsKey, storeId);
        return settings ?? new ShopstrSettings();
    }

    public Task SetStoreSettings(string storeId, ShopstrSettings settings)
    {
        return _settingsRepository.UpdateSettingAsync(SettingsKey, settings, storeId);
    }
}
