using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using WeatherPro.Interfaces;
using WeatherPro.Services;
using WeatherPro.ViewModels;

namespace WeatherPro;

/// <summary>App entry point and composition root. Builds a single <see cref="IServiceProvider"/>
/// used for the lifetime of the process; Views resolve their ViewModels from here in their constructors.</summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public MainWindow? MainWindow { get; private set; }

    public App()
    {
        InitializeComponent();

        var logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WeatherPro", "logs");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(Path.Combine(logFolder, "weatherpro-.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .CreateLogger();

        Services = BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        // Infrastructure
        services.AddSingleton<DatabaseContext>();
        services.AddSingleton<AppState>();
        services.AddSingleton<ISettingsService, SqliteSettingsService>();
        services.AddSingleton<IFavoritesRepository, SqliteFavoritesRepository>();
        services.AddSingleton<IWeatherCacheService, SqliteWeatherCacheService>();

        // HTTP-backed services get typed HttpClients (pooled, sane defaults) via Microsoft.Extensions.Http
        services.AddHttpClient<ILocationSearchService, GeocodingService>(c => c.Timeout = TimeSpan.FromSeconds(10));

        // Weather providers: Open-Meteo is fully implemented; the other two are registered as extension
        // points (see their XML docs) so Settings can already list them and DI resolves cleanly.
        services.AddHttpClient<IWeatherProvider, OpenMeteoWeatherProvider>(c => c.Timeout = TimeSpan.FromSeconds(10));
        services.AddSingleton<IWeatherProvider, OpenWeatherProvider>();
        services.AddSingleton<IWeatherProvider, WeatherApiProvider>();
        services.AddSingleton<WeatherProviderFactory>();

        // ViewModels
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ForecastViewModel>();
        services.AddTransient<FavoritesViewModel>();
        services.AddTransient<SettingsViewModel>();

        return services.BuildServiceProvider();
    }
}
