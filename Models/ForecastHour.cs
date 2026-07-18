namespace WeatherPro.Models;

public sealed record ForecastHour(DateTimeOffset Time, double TemperatureC, int WeatherCode, double RainProbabilityPct, int HumidityPct, double WindSpeedKmh);
