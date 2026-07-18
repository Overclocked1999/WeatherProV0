using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WeatherPro.ViewModels;

namespace WeatherPro.Views;

public sealed partial class ForecastPage : Page
{
    public ForecastViewModel ViewModel { get; }

    public ForecastPage()
    {
        ViewModel = App.Services.GetRequiredService<ForecastViewModel>();
        InitializeComponent();
        Loaded += async (_, _) => await ViewModel.LoadCommand.ExecuteAsync(null);
    }
}
