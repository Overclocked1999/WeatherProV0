using CommunityToolkit.Mvvm.ComponentModel;
using WeatherPro.Interfaces;
using WeatherPro.Models;
using WeatherPro.Services;

namespace WeatherPro.ViewModels;

/// <summary>Backs the Settings page. Each property writes straight through to <see cref="ISettingsService"/>
/// on change, so there's no separate "Save" step — matches Windows 11 Settings app conventions.</summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly WeatherProviderFactory _providerFactory;

    public IReadOnlyList<IWeatherProvider> AvailableProviders { get; }

    [ObservableProperty] private UnitSystem _units;
    [ObservableProperty] private AppTheme _theme;
    [ObservableProperty] private int _refreshIntervalMinutes;
    [ObservableProperty] private string _selectedProviderKey;

    public SettingsViewModel(ISettingsService settings, WeatherProviderFactory providerFactory)
    {
        _settings = settings;
        _providerFactory = providerFactory;
        AvailableProviders = providerFactory.GetAllProviders();

        _units = settings.Units;
        _theme = settings.Theme;
        _refreshIntervalMinutes = settings.RefreshIntervalMinutes;
        _selectedProviderKey = settings.WeatherProviderKey;
    }

    partial void OnUnitsChanged(UnitSystem value) => _settings.Units = value;
    partial void OnThemeChanged(AppTheme value) => _settings.Theme = value;
    partial void OnRefreshIntervalMinutesChanged(int value) => _settings.RefreshIntervalMinutes = value;
    partial void OnSelectedProviderKeyChanged(string value) => _settings.WeatherProviderKey = value;
}
