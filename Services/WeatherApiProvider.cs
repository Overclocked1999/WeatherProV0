using WeatherPro.Interfaces;
using WeatherPro.Models;

namespace WeatherPro.Services;

/// <summary>Extension point: implement against https://www.weatherapi.com/ following the same contract
/// as <see cref="OpenMeteoWeatherProvider"/>. Requires an API key.</summary>
public sealed class WeatherApiProvider : IWeatherProvider
{
    public string ProviderKey => "weatherapi";
    public string DisplayName => "WeatherAPI";
    public bool RequiresApiKey => true;

    public Task<CurrentWeather> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default) =>
        throw new NotImplementedException("WeatherAPI provider not yet implemented. Map /v1/forecast.json to CurrentWeather.");

    public Task<IReadOnlyList<ForecastHour>> GetHourlyForecastAsync(double lat, double lon, int hours, CancellationToken ct = default) =>
        throw new NotImplementedException("WeatherAPI provider not yet implemented.");

    public Task<IReadOnlyList<ForecastDay>> GetDailyForecastAsync(double lat, double lon, int days, CancellationToken ct = default) =>
        throw new NotImplementedException("WeatherAPI provider not yet implemented.");
}
