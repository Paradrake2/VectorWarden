using System;
using System.Globalization;

public static class NumberFormatUtil
{
    /// <summary>
    /// Formats a number in fixed-point until |value| >= 10^(sciExpThreshold), then switches to scientific.
    /// </summary>
    public static string FormatWithSciThreshold(double value, int sciExpThreshold = 20, int decimals = 0, bool useGrouping = false)
    {
        if (value == 0) return "0";
        if (double.IsNaN(value) || double.IsInfinity(value))
            return value.ToString(CultureInfo.InvariantCulture);

        double abs = Math.Abs(value);
        double exp = Math.Floor(Math.Log10(abs));

        // Use scientific only when exponent >= threshold
        if (exp >= sciExpThreshold)
        {
            string sciFmt = (decimals > 0) ? $"0.{new string('#', decimals)}E+0" : "0E+0";
            return value.ToString(sciFmt, CultureInfo.InvariantCulture);
        }

        // Fixed-point
        var ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        if (!useGrouping) ci.NumberFormat.NumberGroupSeparator = "";
        string fixedFmt = (useGrouping ? "N" : "F") + decimals; // e.g., "F0", "N2"
        return value.ToString(fixedFmt, ci);
    }

    // Convenience overload for floats
    public static string FormatWithSciThreshold(float value, int sciExpThreshold = 20, int decimals = 0, bool useGrouping = false)
        => FormatWithSciThreshold((double)value, sciExpThreshold, decimals, useGrouping);
}
