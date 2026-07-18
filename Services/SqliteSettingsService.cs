using Microsoft.Data.Sqlite;
using WeatherPro.Interfaces;
using WeatherPro.Models;

namespace WeatherPro.Services;

/// <summary>Key/value settings backed by SQLite, cached in memory for synchronous property access
/// (WinUI bindings expect settings reads to be instant, not async).</summary>
public sealed class SqliteSettingsService : ISettingsService
{
    private readonly DatabaseContext _db;
    private readonly Dictionary<string, string> _cache = new();
    public event EventHandler? SettingsChanged;

    public SqliteSettingsService(DatabaseContext db)
    {
        _db = db;
        LoadAll();
    }

    public UnitSystem Units
    {
        get => Enum.TryParse<UnitSystem>(Get("units"), out var u) ? u : UnitSystem.Metric;
        set => Set("units", value.ToString());
    }

    public AppTheme Theme
    {
        get => Enum.TryParse<AppTheme>(Get("theme"), out var t) ? t : AppTheme.System;
        set => Set("theme", value.ToString());
    }

    public string WeatherProviderKey
    {
        get => Get("provider") ?? "open-meteo";
        set => Set("provider", value);
    }

    public int RefreshIntervalMinutes
    {
        get => int.TryParse(Get("refresh_minutes"), out var m) ? m : 30;
        set => Set("refresh_minutes", value.ToString());
    }

    public string? ApiKeyFor(string providerKey) => Get($"apikey_{providerKey}");
    public void SetApiKeyFor(string providerKey, string apiKey) => Set($"apikey_{providerKey}", apiKey);

    private string? Get(string key) => _cache.TryGetValue(key, out var v) ? v : null;

    private void Set(string key, string value)
    {
        _cache[key] = value;
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO settings (key, value) VALUES ($k, $v) ON CONFLICT(key) DO UPDATE SET value = $v;";
        cmd.Parameters.AddWithValue("$k", key);
        cmd.Parameters.AddWithValue("$v", value);
        cmd.ExecuteNonQuery();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LoadAll()
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT key, value FROM settings;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            _cache[reader.GetString(0)] = reader.GetString(1);
    }
}
