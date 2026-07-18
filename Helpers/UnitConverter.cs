using WeatherPro.Models;

namespace WeatherPro.Helpers;

/// <summary>Converts stored metric values to the user's preferred display units.</summary>
public static class UnitConverter
{
    public static double Temperature(double celsius, UnitSystem units) =>
        units == UnitSystem.Imperial ? celsius * 9.0 / 5.0 + 32.0 : celsius;

    public static string TemperatureUnit(UnitSystem units) => units == UnitSystem.Imperial ? "°F" : "°C";

    public static double Speed(double kmh, UnitSystem units) =>
        units == UnitSystem.Imperial ? kmh * 0.621371 : kmh;

    public static string SpeedUnit(UnitSystem units) => units == UnitSystem.Imperial ? "mph" : "km/h";

    public static double Distance(double km, UnitSystem units) =>
        units == UnitSystem.Imperial ? km * 0.621371 : km;

    public static string DistanceUnit(UnitSystem units) => units == UnitSystem.Imperial ? "mi" : "km";

    public static string WindCompass(double degrees)
    {
        string[] dirs = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
        int idx = (int)Math.Round(degrees % 360 / 22.5) % 16;
        return dirs[idx];
    }
}
