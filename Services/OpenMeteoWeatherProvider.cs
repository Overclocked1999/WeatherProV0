using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherPro.Interfaces;
using WeatherPro.Models;

namespace WeatherPro.Services;

/// <summary>Fully implemented provider using Open-Meteo's free forecast + air-quality APIs (no API key required).
/// This is the reference implementation the provider abstraction is designed around; other providers
/// (see <see cref="OpenWeatherProvider"/>, <see cref="WeatherApiProvider"/>) implement the same contract.</summary>
public sealed class OpenMeteoWeatherProvider : IWeatherProvider
{
    private const string ForecastBase = "https://api.open-meteo.com/v1/forecast";
    private const string AirQualityBase = "https://air-quality-api.open-meteo.com/v1/air-quality";

    private readonly HttpClient _http;
    public OpenMeteoWeatherProvider(HttpClient http) => _http = http;

    public string ProviderKey => "open-meteo";
    public string DisplayName => "Open-Meteo";
    public bool RequiresApiKey => false;

    public async Task<CurrentWeather> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default)
    {
        var url = $"{ForecastBase}?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}" +
                   "&current=temperature_2m,apparent_temperature,relative_humidity_2m,surface_pressure,wind_speed_10m,wind_direction_10m," +
                   "visibility,cloud_cover,dew_point_2m,uv_index,weather_code" +
                   "&daily=sunrise,sunset&timezone=auto";

        using var doc = await FetchJsonAsync(url, ct);
        var root = doc.RootElement;
        var current = root.GetProperty("current");
        var daily = root.GetProperty("daily");

        int aqi = await TryGetAirQualityIndexAsync(lat, lon, ct);

        return new CurrentWeather
        {
            TemperatureC = current.GetProperty("temperature_2m").GetDouble(),
            FeelsLikeC = current.GetProperty("apparent_temperature").GetDouble(),
            HumidityPct = current.GetProperty("relative_humidity_2m").GetInt32(),
            PressureHpa = current.GetProperty("surface_pressure").GetDouble(),
            WindSpeedKmh = current.GetProperty("wind_speed_10m").GetDouble(),
            WindDirectionDeg = current.GetProperty("wind_direction_10m").GetDouble(),
            VisibilityKm = current.GetProperty("visibility").GetDouble() / 1000.0,
            CloudCoverPct = current.GetProperty("cloud_cover").GetInt32(),
            DewPointC = current.GetProperty("dew_point_2m").GetDouble(),
            UvIndex = current.TryGetProperty("uv_index", out var uv) ? uv.GetDouble() : 0,
            AirQualityIndex = aqi,
            WeatherCode = current.GetProperty("weather_code").GetInt32(),
            Sunrise = DateTimeOffset.Parse(daily.GetProperty("sunrise")[0].GetString()!),
            Sunset = DateTimeOffset.Parse(daily.GetProperty("sunset")[0].GetString()!),
            LastUpdated = DateTimeOffset.Now,
        };
    }

    public async Task<IReadOnlyList<ForecastHour>> GetHourlyForecastAsync(double lat, double lon, int hours, CancellationToken ct = default)
    {
        var url = $"{ForecastBase}?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}" +
                   "&hourly=temperature_2m,weather_code,precipitation_probability,relative_humidity_2m,wind_speed_10m&timezone=auto" +
                   $"&forecast_hours={Math.Clamp(hours, 1, 384)}";

        using var doc = await FetchJsonAsync(url, ct);
        var hourly = doc.RootElement.GetProperty("hourly");
        var times = hourly.GetProperty("time");
        var temps = hourly.GetProperty("temperature_2m");
        var codes = hourly.GetProperty("weather_code");
        var rain = hourly.GetProperty("precipitation_probability");
        var humidity = hourly.GetProperty("relative_humidity_2m");
        var wind = hourly.GetProperty("wind_speed_10m");

        var result = new List<ForecastHour>(times.GetArrayLength());
        for (int i = 0; i < times.GetArrayLength(); i++)
        {
            result.Add(new ForecastHour(
                Time: DateTimeOffset.Parse(times[i].GetString()!),
                TemperatureC: temps[i].GetDouble(),
                WeatherCode: codes[i].GetInt32(),
                RainProbabilityPct: rain[i].GetDouble(),
                HumidityPct: humidity[i].GetInt32(),
                WindSpeedKmh: wind[i].GetDouble()));
        }
        return result;
    }

    public async Task<IReadOnlyList<ForecastDay>> GetDailyForecastAsync(double lat, double lon, int days, CancellationToken ct = default)
    {
        var url = $"{ForecastBase}?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}" +
                   "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max,wind_speed_10m_max,sunrise,sunset&timezone=auto" +
                   $"&forecast_days={Math.Clamp(days, 1, 16)}";

        using var doc = await FetchJsonAsync(url, ct);
        var daily = doc.RootElement.GetProperty("daily");
        var dates = daily.GetProperty("time");
        var codes = daily.GetProperty("weather_code");
        var tmax = daily.GetProperty("temperature_2m_max");
        var tmin = daily.GetProperty("temperature_2m_min");
        var rain = daily.GetProperty("precipitation_probability_max");
        var windMax = daily.GetProperty("wind_speed_10m_max");
        var sunrise = daily.GetProperty("sunrise");
        var sunset = daily.GetProperty("sunset");

        var result = new List<ForecastDay>(dates.GetArrayLength());
        for (int i = 0; i < dates.GetArrayLength(); i++)
        {
            result.Add(new ForecastDay(
                Date: DateOnly.Parse(dates[i].GetString()!),
                TempMaxC: tmax[i].GetDouble(),
                TempMinC: tmin[i].GetDouble(),
                WeatherCode: codes[i].GetInt32(),
                RainProbabilityPct: rain[i].GetDouble(),
                WindSpeedMaxKmh: windMax[i].GetDouble(),
                Sunrise: DateTimeOffset.Parse(sunrise[i].GetString()!),
                Sunset: DateTimeOffset.Parse(sunset[i].GetString()!)));
        }
        return result;
    }

    private async Task<int> TryGetAirQualityIndexAsync(double lat, double lon, CancellationToken ct)
    {
        try
        {
            var url = $"{AirQualityBase}?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}&current=us_aqi";
            using var doc = await FetchJsonAsync(url, ct);
            return doc.RootElement.GetProperty("current").GetProperty("us_aqi").GetInt32();
        }
        catch
        {
            return 0; // AQI is a supplementary field; failures here shouldn't break the main weather fetch
        }
    }

    private async Task<JsonDocument> FetchJsonAsync(string url, CancellationToken ct)
    {
        using var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync(ct);
        return await JsonDocument.ParseAsync(stream, cancellationToken: ct);
    }
}
