using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using WeatherPro.Interfaces;
using WeatherPro.Models;
using WeatherPro.Services;

namespace WeatherPro.ViewModels;

/// <summary>Backs the 7-day / extended forecast page.</summary>
public sealed partial class ForecastViewModel : ObservableObject
{
    private readonly WeatherProviderFactory _providerFactory;
    private readonly AppState _appState;

    [ObservableProperty] private IReadOnlyList<ForecastDay> _days = Array.Empty<ForecastDay>();
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private int _forecastLength = 7;

    public ForecastViewModel(WeatherProviderFactory providerFactory, AppState appState)
    {
        _providerFactory = providerFactory;
        _appState = appState;
        _appState.CurrentLocationChanged += async (_, _) => await LoadAsync();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var provider = _providerFactory.GetActiveProvider();
            var loc = _appState.CurrentLocation;
            Days = await provider.GetDailyForecastAsync(loc.Latitude, loc.Longitude, ForecastLength);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Couldn't load the extended forecast.";
            Log.Error(ex, "Forecast load failed");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
