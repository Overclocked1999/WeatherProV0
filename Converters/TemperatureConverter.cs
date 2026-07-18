using Microsoft.UI.Xaml.Data;
using WeatherPro.Helpers;

namespace WeatherPro.Converters;

/// <summary>Converts a stored Celsius double to a display string in the user's preferred unit.
/// ConverterParameter is a format string; use '{0}' as the value placeholder, e.g. "Feels like {0}".
/// Reads the live unit preference from settings on every call so it reacts immediately to Settings changes.</summary>
public sealed class TemperatureConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not double celsius) return "--";
        var units = App.Services.GetService(typeof(Interfaces.ISettingsService)) is Interfaces.ISettingsService s ? s.Units : Models.UnitSystem.Metric;
        var converted = UnitConverter.Temperature(celsius, units);
        var unitSymbol = UnitConverter.TemperatureUnit(units);
        var display = $"{converted:F0}{unitSymbol}";

        var format = parameter as string;
        return string.IsNullOrEmpty(format) || format == "0" ? display : string.Format(format, display);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
