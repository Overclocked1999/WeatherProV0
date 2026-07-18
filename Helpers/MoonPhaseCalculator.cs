namespace WeatherPro.Helpers;

/// <summary>Approximates the current moon phase using a synodic-month reference calculation.</summary>
public static class MoonPhaseCalculator
{
    private static readonly DateTime KnownNewMoon = new(2000, 1, 6, 18, 14, 0, DateTimeKind.Utc);
    private const double SynodicMonthDays = 29.53058867;

    public static double GetPhaseFraction(DateTime utcNow)
    {
        double days = (utcNow - KnownNewMoon).TotalDays;
        double phase = days % SynodicMonthDays;
        if (phase < 0) phase += SynodicMonthDays;
        return phase / SynodicMonthDays; // 0 = new moon, 0.5 = full moon
    }

    public static string GetPhaseName(DateTime utcNow)
    {
        double f = GetPhaseFraction(utcNow);
        return f switch
        {
            < 0.03 or >= 0.97 => "New Moon",
            < 0.22 => "Waxing Crescent",
            < 0.28 => "First Quarter",
            < 0.47 => "Waxing Gibbous",
            < 0.53 => "Full Moon",
            < 0.72 => "Waning Gibbous",
            < 0.78 => "Last Quarter",
            _ => "Waning Crescent",
        };
    }
}
