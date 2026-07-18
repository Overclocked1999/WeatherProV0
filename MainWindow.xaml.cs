using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WeatherPro.Views;

namespace WeatherPro;

/// <summary>Shell window: a NavigationView + Frame that swaps between the four main pages.
/// Mica backdrop gives the Windows 11 look for free via SystemBackdrop.</summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SystemBackdrop = new MicaBackdrop();
        Title = "WeatherPro";
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e) => ContentFrame.Navigate(typeof(DashboardPage));

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var tag = (args.SelectedItemContainer as NavigationViewItem)?.Tag as string;
        Type pageType = tag switch
        {
            "forecast" => typeof(ForecastPage),
            "favorites" => typeof(FavoritesPage),
            "settings" => typeof(SettingsPage),
            _ => typeof(DashboardPage),
        };
        if (ContentFrame.CurrentSourcePageType != pageType)
            ContentFrame.Navigate(pageType);
    }
}
