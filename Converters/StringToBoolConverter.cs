using Microsoft.UI.Xaml.Data;

namespace WeatherPro.Converters;

/// <summary>True when the bound string is non-empty; used to drive InfoBar.IsOpen from an error-message property.</summary>
public sealed class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        !string.IsNullOrEmpty(value as string);

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
