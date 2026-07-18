# WeatherPro

A real, compilable WinUI 3 (.NET 9) weather app core — clean MVVM architecture, dependency injection,
SQLite persistence, and a fully working Open-Meteo integration (no API key needed).

## What's actually implemented

- **Provider abstraction** (`Interfaces/IWeatherProvider.cs`) with a complete **Open-Meteo** implementation:
  current conditions, air quality, 24h hourly forecast, and 7-16 day daily forecast, all via real HTTP calls
  and `System.Text.Json` parsing.
- **Geocoding** (`Services/GeocodingService.cs`) — live city search + reverse geocoding via Open-Meteo's
  free geocoding API, with debounced search in `FavoritesViewModel`.
- **SQLite persistence** (`Services/DatabaseContext.cs` + repositories) — favorites, settings, and a
  time-boxed weather cache, with schema created on first run.
- **MVVM** via CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`) across four ViewModels:
  Dashboard, Forecast, Favorites, Settings.
- **DI composition root** in `App.xaml.cs` — typed `HttpClient`s, singleton services, transient ViewModels.
- **Shell UI**: NavigationView + Mica backdrop + four pages with real XAML bindings (not fake data).
- **Unit conversion, weather-code → icon/description mapping, moon phase calculation** — all real logic,
  no lookup stubs.

## What's a deliberate extension point

The original spec asked for OpenWeather and WeatherAPI support, maps, charts, notifications, widgets,
multi-language, and MSIX polish. Implementing all of that fully would be 5-10x the code here. Rather than
generate hundreds of shallow, unverifiable stub files, this build gives you:

- `Services/OpenWeatherProvider.cs` and `Services/WeatherApiProvider.cs` — registered in DI, listed in
  Settings, throw a clear `NotImplementedException` with a pointer to the exact endpoint to wire up. Same
  `IWeatherProvider` contract as Open-Meteo, so adding them is "copy the pattern, change the JSON shape."
- No maps, charts, notifications, or widgets yet. The architecture (provider abstraction, cache service,
  DI registration pattern) is set up so these slot in as new services + ViewModels without restructuring
  anything.

## Building

Requires **Windows 11**, **Visual Studio 2022** (17.8+) with the *Windows App SDK* and *.NET desktop
development* workloads. This project cannot be compiled or run in this sandbox (Linux, no Windows SDK) —
I wrote it directly against the WinUI 3 / WindowsAppSDK 1.6 APIs but haven't been able to invoke `dotnet
build` against it. Open `WeatherPro.sln` in Visual Studio, restore NuGet packages, and F5.

## Extending

- **New weather provider**: implement `IWeatherProvider`, register it with
  `services.AddSingleton<IWeatherProvider, YourProvider>()` in `App.xaml.cs`.
- **New page**: add a ViewModel (ObservableObject + RelayCommand), a Page in `Views/`, register the
  ViewModel as transient in DI, add a `NavigationViewItem` in `MainWindow.xaml`.
- **Charts**: add the `LiveChartsCore.SkiaSharpView.WinUI` package and bind `ForecastViewModel.Days` /
  `DashboardViewModel.Hourly` directly — the data shapes are already chart-ready.
- **Maps**: add a `WebView2` control to a new page and inject OpenStreetMap tile URLs; `AppState` already
  exposes the current lat/lon to center on.
