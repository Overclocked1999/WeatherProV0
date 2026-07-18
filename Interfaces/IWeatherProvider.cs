using WeatherPro.Models;

namespace WeatherPro.Interfaces;

/// <summary>Abstraction over a weather data source (Open-Meteo, OpenWeather, WeatherAPI, ...).
/// Implementations are selected at runtime via <see cref="Services.WeatherProviderFactory"/>.</summary>
public interface IWeatherProvider
{
    string ProviderKey { get; }
    string DisplayName { get; }
    bool RequiresApiKey { get; }

    Task<CurrentWeather> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default);
    Task<IReadOnlyList<ForecastHour>> GetHourlyForecastAsync(double lat, double lon, int hours, CancellationToken ct = default);
    Task<IReadOnlyList<ForecastDay>> GetDailyForecastAsync(double lat, double lon, int days, CancellationToken ct = default);
}
