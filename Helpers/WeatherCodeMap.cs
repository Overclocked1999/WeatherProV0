namespace WeatherPro.Helpers;

/// <summary>Maps Open-Meteo WMO weather codes to descriptions and Segoe Fluent icon glyphs.</summary>
public static class WeatherCodeMap
{
    // WMO code -> (description, day glyph, night glyph)
    private static readonly Dictionary<int, (string Desc, string Day, string Night)> Map = new()
    {
        [0]  = ("Clear sky",        "\uE706", "\uE708"),
        [1]  = ("Mainly clear",     "\uE706", "\uE708"),
        [2]  = ("Partly cloudy",    "\uE753", "\uE753"),
        [3]  = ("Overcast",         "\uE752", "\uE752"),
        [45] = ("Fog",              "\uE74F", "\uE74F"),
        [48] = ("Rime fog",         "\uE74F", "\uE74F"),
        [51] = ("Light drizzle",    "\uE712", "\uE712"),
        [53] = ("Drizzle",          "\uE712", "\uE712"),
        [55] = ("Dense drizzle",    "\uE712", "\uE712"),
        [61] = ("Slight rain",      "\uE712", "\uE712"),
        [63] = ("Rain",             "\uE712", "\uE712"),
        [65] = ("Heavy rain",       "\uE714", "\uE714"),
        [71] = ("Slight snow",      "\uE717", "\uE717"),
        [73] = ("Snow",             "\uE717", "\uE717"),
        [75] = ("Heavy snow",       "\uE717", "\uE717"),
        [80] = ("Rain showers",     "\uE712", "\uE712"),
        [81] = ("Rain showers",     "\uE712", "\uE712"),
        [82] = ("Violent showers",  "\uE714", "\uE714"),
        [95] = ("Thunderstorm",     "\uE76C", "\uE76C"),
        [96] = ("Thunderstorm, hail","\uE76C", "\uE76C"),
        [99] = ("Severe thunderstorm","\uE76C","\uE76C"),
    };

    public static string Describe(int code) => Map.TryGetValue(code, out var v) ? v.Desc : "Unknown";

    public static string Glyph(int code, bool isDaytime)
    {
        if (!Map.TryGetValue(code, out var v)) return "\uE753";
        return isDaytime ? v.Day : v.Night;
    }
}
