using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WeatherPro.ViewModels;

namespace WeatherPro.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    // Simple label/value pairs for the secondary-metrics GridView; kept as plain objects rather than a
    // full model since this projection only exists for display and would otherwise duplicate CurrentWeather.
    public IReadOnlyList<MetricTile> MetricTiles => ViewModel.Current is null
        ? Array.Empty<MetricTile>()
        : new[]
        {
            new MetricTile("Humidity", $"{ViewModel.Current.HumidityPct}%"),
            new MetricTile("Wind", $"{ViewModel.Current.WindSpeedKmh:F0} km/h"),
            new MetricTile("Pressure", $"{ViewModel.Current.PressureHpa:F0} hPa"),
            new MetricTile("UV Index", $"{ViewModel.Current.UvIndex:F0}"),
            new MetricTile("Air Quality", $"{ViewModel.Current.AirQualityIndex}"),
            new MetricTile("Visibility", $"{ViewModel.Current.VisibilityKm:F1} km"),
            new MetricTile("Dew Point", $"{ViewModel.Current.DewPointC:F0}°"),
            new MetricTile("Cloud Cover", $"{ViewModel.Current.CloudCoverPct}%"),
            new MetricTile("Sunrise", ViewModel.Current.Sunrise.ToString("t")),
            new MetricTile("Sunset", ViewModel.Current.Sunset.ToString("t")),
            new MetricTile("Moon Phase", ViewModel.Current.MoonPhase),
        };

    public DashboardPage()
    {
        ViewModel = App.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();
        Loaded += async (_, _) => await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}

public sealed record MetricTile(string Label, string Value);
