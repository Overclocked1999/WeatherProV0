using WeatherPro.Models;

namespace WeatherPro.Interfaces;

public interface ILocationSearchService
{
    Task<IReadOnlyList<GeoLocation>> SearchAsync(string query, CancellationToken ct = default);
    Task<GeoLocation?> ReverseGeocodeAsync(double lat, double lon, CancellationToken ct = default);
}
