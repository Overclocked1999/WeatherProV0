namespace WeatherPro.Interfaces;

/// <summary>Simple time-boxed cache to avoid redundant API calls (in-memory, backed by SQLite for persistence across launches).</summary>
public interface IWeatherCacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan ttl) where T : class;
}
