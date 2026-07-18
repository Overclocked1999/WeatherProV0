using WeatherPro.Models;

namespace WeatherPro.Interfaces;

/// <summary>Persists lightweight user preferences as key/value pairs in SQLite.</summary>
public interface ISettingsService
{
    UnitSystem Units { get; set; }
    AppTheme Theme { get; set; }
    string WeatherProviderKey { get; set; }
    int RefreshIntervalMinutes { get; set; }
    string? ApiKeyFor(string providerKey);
    void SetApiKeyFor(string providerKey, string apiKey);
    event EventHandler? SettingsChanged;
}
