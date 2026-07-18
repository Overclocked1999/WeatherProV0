using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using WeatherPro.Interfaces;
using WeatherPro.Models;
using WeatherPro.Services;

namespace WeatherPro.ViewModels;

/// <summary>Backs the main dashboard: current conditions + next-24h strip for whatever location
/// is active in <see cref="AppState"/>.</summary>
public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly WeatherProviderFactory _providerFactory;
    private readonly IWeatherCacheService _cache;
    private readonly ISettingsService _settings;
    private readonly AppState _appState;

    [ObservableProperty] private CurrentWeather? _current;
    [ObservableProperty] private IReadOnlyList<ForecastHour> _hourly = Array.Empty<ForecastHour>();
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string _locationLabel = "";

    public UnitSystem Units => _settings.Units;

    public DashboardViewModel(WeatherProviderFactory providerFactory, IWeatherCacheService cache, ISettingsService settings, AppState appState)
    {
        _providerFactory = providerFactory;
        _cache = cache;
        _settings = settings;
        _appState = appState;
        _appState.CurrentLocationChanged += async (_, loc) => await RefreshAsync(loc);
        LocationLabel = _appState.CurrentLocation.DisplayName;
    }

    [RelayCommand]
    public async Task LoadAsync() => await RefreshAsync(_appState.CurrentLocation);

    private async Task RefreshAsync(GeoLocation location)
    {
        IsLoading = true;
        ErrorMessage = null;
        LocationLabel = location.DisplayName;
        var cacheKey = $"current_{location.Latitude:F2}_{location.Longitude:F2}";

        try
        {
            var provider = _providerFactory.GetActiveProvider();

            // Serve cached data instantly if fresh, then still refresh in the background on manual reload.
            Current = await _cache.GetAsync<CurrentWeather>(cacheKey)
                       ?? await FetchAndCacheCurrentAsync(provider, location, cacheKey);

            Hourly = await provider.GetHourlyForecastAsync(location.Latitude, location.Longitude, 24);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = "Couldn't reach the weather service. Showing last known data if available.";
            Log.Warning(ex, "Dashboard refresh failed (network)");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Something went wrong loading the forecast.";
            Log.Error(ex, "Dashboard refresh failed");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<CurrentWeather> FetchAndCacheCurrentAsync(IWeatherProvider provider, GeoLocation location, string cacheKey)
    {
        var fresh = await provider.GetCurrentWeatherAsync(location.Latitude, location.Longitude);
        await _cache.SetAsync(cacheKey, fresh, TimeSpan.FromMinutes(_settings.RefreshIntervalMinutes));
        return fresh;
    }
}
