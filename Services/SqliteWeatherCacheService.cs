using System.Text.Json;
using WeatherPro.Interfaces;

namespace WeatherPro.Services;

/// <summary>Caches provider responses to disk so the app can start in offline mode with last-known data,
/// and to avoid re-fetching within the TTL window.</summary>
public sealed class SqliteWeatherCacheService : IWeatherCacheService
{
    private readonly DatabaseContext _db;
    public SqliteWeatherCacheService(DatabaseContext db) => _db = db;

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT payload, expires_at FROM weather_cache WHERE cache_key = $k;";
        cmd.Parameters.AddWithValue("$k", key);
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var payload = reader.GetString(0);
        var expiresAt = DateTimeOffset.Parse(reader.GetString(1));
        if (expiresAt < DateTimeOffset.UtcNow) return null; // stale; caller will re-fetch and overwrite

        return JsonSerializer.Deserialize<T>(payload);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl) where T : class
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO weather_cache (cache_key, payload, expires_at) VALUES ($k, $p, $e) ON CONFLICT(cache_key) DO UPDATE SET payload = $p, expires_at = $e;";
        cmd.Parameters.AddWithValue("$k", key);
        cmd.Parameters.AddWithValue("$p", JsonSerializer.Serialize(value));
        cmd.Parameters.AddWithValue("$e", DateTimeOffset.UtcNow.Add(ttl).ToString("O"));
        await cmd.ExecuteNonQueryAsync();
    }
}
