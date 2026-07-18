using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherPro.Interfaces;
using WeatherPro.Models;
using WeatherPro.Services;

namespace WeatherPro.ViewModels;

/// <summary>Backs the Favorites page: saved locations plus the search-and-add flow.
/// Also doubles as the multi-location dashboard data source.</summary>
public sealed partial class FavoritesViewModel : ObservableObject
{
    private readonly IFavoritesRepository _favorites;
    private readonly ILocationSearchService _search;
    private readonly AppState _appState;

    public ObservableCollection<FavoriteLocation> Favorites { get; } = new();
    public ObservableCollection<GeoLocation> SearchResults { get; } = new();

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private bool _isSearching;

    private CancellationTokenSource? _searchCts;

    public FavoritesViewModel(IFavoritesRepository favorites, ILocationSearchService search, AppState appState)
    {
        _favorites = favorites;
        _search = search;
        _appState = appState;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Favorites.Clear();
        foreach (var f in await _favorites.GetAllAsync())
            Favorites.Add(f);
    }

    partial void OnSearchTextChanged(string value) => _ = DebouncedSearchAsync(value);

    private async Task DebouncedSearchAsync(string query)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            await Task.Delay(300, token); // debounce so we don't spam the API on every keystroke
            IsSearching = true;
            var results = await _search.SearchAsync(query, token);
            if (token.IsCancellationRequested) return;

            SearchResults.Clear();
            foreach (var r in results) SearchResults.Add(r);
        }
        catch (TaskCanceledException) { /* expected when a newer keystroke supersedes this search */ }
        finally
        {
            if (!token.IsCancellationRequested) IsSearching = false;
        }
    }

    [RelayCommand]
    public async Task AddFavoriteAsync(GeoLocation location)
    {
        var added = await _favorites.AddAsync(location);
        Favorites.Add(added);
        SearchResults.Clear();
        SearchText = "";
    }

    [RelayCommand]
    public async Task RemoveFavoriteAsync(FavoriteLocation favorite)
    {
        await _favorites.RemoveAsync(favorite.Id);
        Favorites.Remove(favorite);
    }

    [RelayCommand]
    public async Task RenameFavoriteAsync((FavoriteLocation Favorite, string NewName) args)
    {
        await _favorites.RenameAsync(args.Favorite.Id, args.NewName);
        args.Favorite.DisplayName = args.NewName;
    }

    [RelayCommand]
    public void SelectFavorite(FavoriteLocation favorite) =>
        _appState.CurrentLocation = new GeoLocation(favorite.DisplayName, "", null, favorite.Latitude, favorite.Longitude);

    [RelayCommand]
    public void SelectSearchResult(GeoLocation location) => _appState.CurrentLocation = location;
}
