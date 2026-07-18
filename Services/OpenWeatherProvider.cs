using WeatherPro.Interfaces;
using WeatherPro.Models;

namespace WeatherPro.Services;

/// <summary>Extension point: implement against https://openweathermap.org/api using the same
/// method shapes as <see cref="OpenMeteoWeatherProvider"/>. Requires an API key (see ISettingsService.ApiKeyFor).
/// Not implemented in this build — registered so it shows in Settings, but selecting it will surface
/// a friendly "not configured" error via WeatherProviderFactory rather than crashing.</summary>
public sealed class OpenWeatherProvider : IWeatherProvider
{
    public string ProviderKey => "openweather";
    public string DisplayName => "OpenWeather";
    public bool RequiresApiKey => true;

    public Task<CurrentWeather> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default) =>
        throw new NotImplementedException("OpenWeather provider not yet implemented. Map /data/3.0/onecall to CurrentWeather.");

    public Task<IReadOnlyList<ForecastHour>> GetHourlyForecastAsync(double lat, double lon, int hours, CancellationToken ct = default) =>
        throw new NotImplementedException("OpenWeather provider not yet implemented.");

    public Task<IReadOnlyList<ForecastDay>> GetDailyForecastAsync(double lat, double lon, int days, CancellationToken ct = default) =>
        throw new NotImplementedException("OpenWeather provider not yet implemented.");
}
