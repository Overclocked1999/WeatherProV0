using Microsoft.Data.Sqlite;
using WeatherPro.Interfaces;
using WeatherPro.Models;

namespace WeatherPro.Services;

public sealed class SqliteFavoritesRepository : IFavoritesRepository
{
    private readonly DatabaseContext _db;
    public SqliteFavoritesRepository(DatabaseContext db) => _db = db;

    public async Task<IReadOnlyList<FavoriteLocation>> GetAllAsync()
    {
        var results = new List<FavoriteLocation>();
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, display_name, latitude, longitude, is_default, sort_order FROM favorites ORDER BY sort_order, display_name;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new FavoriteLocation
            {
                Id = reader.GetInt32(0),
                DisplayName = reader.GetString(1),
                Latitude = reader.GetDouble(2),
                Longitude = reader.GetDouble(3),
                IsDefault = reader.GetInt32(4) == 1,
                SortOrder = reader.GetInt32(5),
            });
        }
        return results;
    }

    public async Task<FavoriteLocation> AddAsync(GeoLocation location)
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO favorites (display_name, latitude, longitude, is_default, sort_order)
            VALUES ($n, $lat, $lon, 0, (SELECT COALESCE(MAX(sort_order), 0) + 1 FROM favorites));
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$n", location.DisplayName);
        cmd.Parameters.AddWithValue("$lat", location.Latitude);
        cmd.Parameters.AddWithValue("$lon", location.Longitude);
        var id = (long)(await cmd.ExecuteScalarAsync() ?? 0L);
        return new FavoriteLocation { Id = (int)id, DisplayName = location.DisplayName, Latitude = location.Latitude, Longitude = location.Longitude };
    }

    public async Task RemoveAsync(int id)
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM favorites WHERE id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RenameAsync(int id, string newName)
    {
        using var conn = _db.OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE favorites SET display_name = $n WHERE id = $id;";
        cmd.Parameters.AddWithValue("$n", newName);
        cmd.Parameters.AddWithValue("$id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetDefaultAsync(int id)
    {
        using var conn = _db.OpenConnection();
        using var tx = conn.BeginTransaction();
        using (var clear = conn.CreateCommand())
        {
            clear.Transaction = tx;
            clear.CommandText = "UPDATE favorites SET is_default = 0;";
            await clear.ExecuteNonQueryAsync();
        }
        using (var set = conn.CreateCommand())
        {
            set.Transaction = tx;
            set.CommandText = "UPDATE favorites SET is_default = 1 WHERE id = $id;";
            set.Parameters.AddWithValue("$id", id);
            await set.ExecuteNonQueryAsync();
        }
        tx.Commit();
    }
}
