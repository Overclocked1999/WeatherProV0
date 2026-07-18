using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WeatherPro.Models;
using WeatherPro.ViewModels;

namespace WeatherPro.Views;

public sealed partial class FavoritesPage : Page
{
    public FavoritesViewModel ViewModel { get; }

    public FavoritesPage()
    {
        ViewModel = App.Services.GetRequiredService<FavoritesViewModel>();
        InitializeComponent();
        Loaded += async (_, _) => await ViewModel.LoadCommand.ExecuteAsync(null);
    }

    private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is GeoLocation location)
            await ViewModel.AddFavoriteCommand.ExecuteAsync(location);
    }

    private void FavoriteItem_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is FavoriteLocation favorite)
            ViewModel.SelectFavoriteCommand.Execute(favorite);
    }

    private async void RemoveFavorite_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is FavoriteLocation favorite)
            await ViewModel.RemoveFavoriteCommand.ExecuteAsync(favorite);
    }
}
