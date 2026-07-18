using System.Text.Json;
using WeatherPro.Interfaces;
using WeatherPro.Models;

namespace WeatherPro.Services;

/// <summary>Location search backed by Open-Meteo's free geocoding API. Used for the search bar's
/// autocomplete/live suggestions and for adding new favorites.</summary>
public sealed class GeocodingService : ILocationSearchService
{
    private const string SearchUrl = "https://geocoding-api.open-meteo.com/v1/search";
    private const string ReverseUrl = "https://geocoding-api.open-meteo.com/v1/reverse";
    private readonly HttpClient _http;

    public GeocodingService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<GeoLocation>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return Array.Empty<GeoLocation>();

        var url = $"{SearchUrl}?name={Uri.EscapeDataString(query.Trim())}&count=8&language=en&format=json";
        using var response = await _http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return Array.Empty<GeoLocation>();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
        if (!doc.RootElement.TryGetProperty("results", out var results)) return Array.Empty<GeoLocation>();

        var list = new List<GeoLocation>();
        foreach (var item in results.EnumerateArray())
        {
            list.Add(new GeoLocation(
                Name: item.GetProperty("name").GetString() ?? "",
                Country: item.TryGetProperty("country", out var c) ? c.GetString() ?? "" : "",
                Admin1: item.TryGetProperty("admin1", out var a) ? a.GetString() : null,
                Latitude: item.GetProperty("latitude").GetDouble(),
                Longitude: item.GetProperty("longitude").GetDouble()));
        }
        return list;
    }

    public async Task<GeoLocation?> ReverseGeocodeAsync(double lat, double lon, CancellationToken ct = default)
    {
        // Open-Meteo's reverse endpoint availability varies by region; fall back to coordinate label on failure.
        try
        {
            var url = $"{ReverseUrl}?latitude={lat}&longitude={lon}&language=en&format=json";
            using var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return Fallback();
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
            if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0) return Fallback();
            var item = results[0];
            return new GeoLocation(
                item.GetProperty("name").GetString() ?? "Current Location",
                item.TryGetProperty("country", out var c) ? c.GetString() ?? "" : "",
                item.TryGetProperty("admin1", out var a) ? a.GetString() : null,
                lat, lon);
        }
        catch
        {
            return Fallback();
        }

        GeoLocation Fallback() => new("Current Location", "", null, lat, lon);
    }
}
