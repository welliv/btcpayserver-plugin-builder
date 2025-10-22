using System.ComponentModel.DataAnnotations;

namespace BTCPayServer.Plugins.Shopstr.Models;

public class ShopstrSettings
{
    [Display(Name = "Nostr public key")]
    public string? PublicKey { get; set; }

    [Display(Name = "Nostr private key")]
    public string? PrivateKey { get; set; }

    [Display(Name = "Preferred relays (comma separated)")]
    public string? RelayList { get; set; }

    [Display(Name = "Automatically sync product listings")]
    public bool AutoPublish { get; set; } = true;

    [Display(Name = "Enable encrypted customer messaging")]
    public bool MessagingEnabled { get; set; } = true;

    public IReadOnlyList<string> GetRelayUris()
    {
        if (string.IsNullOrWhiteSpace(RelayList))
        {
            return Array.Empty<string>();
        }

        return RelayList
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
