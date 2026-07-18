namespace WeatherPro.Models;

public enum AlertSeverity { Info, Warning, Severe }

public sealed record WeatherAlert(string Title, string Description, AlertSeverity Severity, DateTimeOffset IssuedAt);
