namespace WeatherPro.Models;

/// <summary>A resolved place from geocoding search or a saved favorite/current location.</summary>
public sealed record GeoLocation(string Name, string Country, string? Admin1, double Latitude, double Longitude)
{
    public string DisplayName => string.IsNullOrEmpty(Admin1) ? $"{Name}, {Country}" : $"{Name}, {Admin1}, {Country}";
}
