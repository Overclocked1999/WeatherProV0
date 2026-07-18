namespace WeatherPro.Models;

public sealed record ForecastDay(DateOnly Date, double TempMaxC, double TempMinC, int WeatherCode, double RainProbabilityPct, double WindSpeedMaxKmh, DateTimeOffset Sunrise, DateTimeOffset Sunset);
