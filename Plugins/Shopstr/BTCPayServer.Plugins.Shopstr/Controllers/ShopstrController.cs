using BTCPayServer.Plugins.Shopstr.Models;
using BTCPayServer.Plugins.Shopstr.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Plugins.Shopstr.Controllers;

[Authorize]
[Route("stores/{storeId}/plugins/shopstr")]
public class ShopstrController : Controller
{
    private readonly ShopstrSettingsRepository _settingsRepository;
    private readonly ShopstrRelayCoordinator _relayCoordinator;

    public ShopstrController(
        ShopstrSettingsRepository settingsRepository,
        ShopstrRelayCoordinator relayCoordinator)
    {
        _settingsRepository = settingsRepository;
        _relayCoordinator = relayCoordinator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string storeId)
    {
        var settings = await _settingsRepository.GetStoreSettings(storeId);
        ViewData["RelayList"] = _relayCoordinator.NormalizeRelays(settings);
        ViewData["StoreId"] = storeId;
        return View("~/Views/Store/Shopstr/Index.cshtml", settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string storeId, ShopstrSettings settings)
    {
        if (!ModelState.IsValid)
        {
            ViewData["RelayList"] = _relayCoordinator.NormalizeRelays(settings);
            ViewData["StoreId"] = storeId;
            return View("~/Views/Store/Shopstr/Index.cshtml", settings);
        }

        await _settingsRepository.SetStoreSettings(storeId, settings);
        TempData["SuccessMessage"] = "Shopstr settings updated.";
        return RedirectToAction(nameof(Index), new { storeId });
    }
}
