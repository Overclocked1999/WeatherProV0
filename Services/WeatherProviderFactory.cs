using WeatherPro.Interfaces;

namespace WeatherPro.Services;

/// <summary>Resolves the active <see cref="IWeatherProvider"/> from settings. All providers are registered
/// in DI; this factory just picks the one whose <see cref="IWeatherProvider.ProviderKey"/> matches the
/// user's setting, defaulting to Open-Meteo if the setting is missing or invalid.</summary>
public sealed class WeatherProviderFactory
{
    private readonly IEnumerable<IWeatherProvider> _providers;
    private readonly ISettingsService _settings;

    public WeatherProviderFactory(IEnumerable<IWeatherProvider> providers, ISettingsService settings)
    {
        _providers = providers;
        _settings = settings;
    }

    public IWeatherProvider GetActiveProvider()
    {
        var key = _settings.WeatherProviderKey;
        return _providers.FirstOrDefault(p => p.ProviderKey == key)
               ?? _providers.First(p => p.ProviderKey == "open-meteo");
    }

    public IReadOnlyList<IWeatherProvider> GetAllProviders() => _providers.ToList();
}
