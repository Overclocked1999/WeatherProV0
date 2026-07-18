using Microsoft.Data.Sqlite;
using Serilog;

namespace WeatherPro.Services;

/// <summary>Owns the SQLite connection string and schema creation. Registered as a singleton;
/// individual repositories open short-lived connections per call (SQLite is fast to open locally).</summary>
public sealed class DatabaseContext
{
    public string ConnectionString { get; }

    public DatabaseContext()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WeatherPro");
        Directory.CreateDirectory(folder);
        var dbPath = Path.Combine(folder, "weatherpro.db");
        ConnectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
        Initialize();
    }

    public SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    private void Initialize()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS favorites (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                display_name TEXT NOT NULL,
                latitude REAL NOT NULL,
                longitude REAL NOT NULL,
                is_default INTEGER NOT NULL DEFAULT 0,
                sort_order INTEGER NOT NULL DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS settings (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS search_history (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                query TEXT NOT NULL,
                searched_at TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS weather_cache (
                cache_key TEXT PRIMARY KEY,
                payload TEXT NOT NULL,
                expires_at TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS weather_history (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                latitude REAL NOT NULL,
                longitude REAL NOT NULL,
                recorded_at TEXT NOT NULL,
                temperature_c REAL NOT NULL,
                wind_kmh REAL NOT NULL,
                rain_mm REAL NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
        Log.Debug("Database schema ensured at {Path}", conn.DataSource);
    }
}
