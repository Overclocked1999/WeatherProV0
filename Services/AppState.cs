using WeatherPro.Models;

namespace WeatherPro.Services;

/// <summary>Holds the currently selected location so Dashboard, Forecast and History pages stay in sync
/// without coupling their view models to each other. Deliberately minimal: a real app might use a
/// message bus (e.g. CommunityToolkit.Mvvm.Messaging) for more complex cross-VM notifications.</summary>
public sealed class AppState
{
    public event EventHandler<GeoLocation>? CurrentLocationChanged;

    private GeoLocation _currentLocation = new("Thessaloniki", "Greece", "Central Macedonia", 40.6401, 22.9444);
    public GeoLocation CurrentLocation
    {
        get => _currentLocation;
        set
        {
            if (_currentLocation == value) return;
            _currentLocation = value;
            CurrentLocationChanged?.Invoke(this, value);
        }
    }
}
