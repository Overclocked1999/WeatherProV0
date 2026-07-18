using WeatherPro.Models;

namespace WeatherPro.Interfaces;

public interface IFavoritesRepository
{
    Task<IReadOnlyList<FavoriteLocation>> GetAllAsync();
    Task<FavoriteLocation> AddAsync(GeoLocation location);
    Task RemoveAsync(int id);
    Task RenameAsync(int id, string newName);
    Task SetDefaultAsync(int id);
}
