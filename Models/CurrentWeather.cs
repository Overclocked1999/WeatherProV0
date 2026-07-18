using WeatherPro.Helpers;

namespace WeatherPro.Models;

/// <summary>Snapshot of current conditions for a location.</summary>
public sealed class CurrentWeather
{
    public double TemperatureC { get; init; }
    public double FeelsLikeC { get; init; }
    public int HumidityPct { get; init; }
    public double PressureHpa { get; init; }
    public double WindSpeedKmh { get; init; }
    public double WindDirectionDeg { get; init; }
    public double VisibilityKm { get; init; }
    public int CloudCoverPct { get; init; }
    public double DewPointC { get; init; }
    public double UvIndex { get; init; }
    public int AirQualityIndex { get; init; }
    public DateTimeOffset Sunrise { get; init; }
    public DateTimeOffset Sunset { get; init; }
    public int WeatherCode { get; init; }
    public DateTimeOffset LastUpdated { get; init; } = DateTimeOffset.Now;

    public string WeatherDescription => WeatherCodeMap.Describe(WeatherCode);
    public string IconGlyph => WeatherCodeMap.Glyph(WeatherCode, IsDaytime);
    public bool IsDaytime => DateTimeOffset.Now is var now && now >= Sunrise && now <= Sunset;
    public string MoonPhase => MoonPhaseCalculator.GetPhaseName(DateTime.UtcNow);
}
