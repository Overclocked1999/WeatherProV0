using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WeatherPro.Models;
using WeatherPro.ViewModels;

namespace WeatherPro.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }
    public UnitSystem[] AllUnitSystems { get; } = Enum.GetValues<UnitSystem>();
    public AppTheme[] AllThemes { get; } = Enum.GetValues<AppTheme>();

    public SettingsPage()
    {
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
        InitializeComponent();
    }
}
