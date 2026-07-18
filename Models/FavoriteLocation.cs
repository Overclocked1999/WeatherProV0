namespace WeatherPro.Models;

/// <summary>A user-saved location, persisted in SQLite.</summary>
public sealed class FavoriteLocation
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
}
