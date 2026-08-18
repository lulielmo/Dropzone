using System.Globalization;
using System.Text.RegularExpressions;

namespace Dropzone.Services;

/// <summary>
/// Extracts a billing period as YYYYMM from dropped Medius PDF text.
/// </summary>
public static class BillingPeriodParser
{
    private static readonly Regex PeriodRange = new(
        @"Period\s+(\d{4})-(\d{2})-(\d{2})\s*--\s*\d{4}-\d{2}-\d{2}",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex PeriodYearMonth = new(
        @"Period\s+(\d{4})-(\d{2})(?!\d)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex BareYearMonth = new(
        @"\b(\d{4})(\d{2})\b",
        RegexOptions.CultureInvariant);

    public static bool TryParse(string? text, out string yearMonth)
    {
        yearMonth = string.Empty;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var range = PeriodRange.Match(text);
        if (range.Success)
            return TryFormat(range.Groups[1].Value, range.Groups[2].Value, out yearMonth);

        var yearMonthMatch = PeriodYearMonth.Match(text);
        if (yearMonthMatch.Success)
            return TryFormat(yearMonthMatch.Groups[1].Value, yearMonthMatch.Groups[2].Value, out yearMonth);

        var bare = BareYearMonth.Match(text);
        if (bare.Success)
            return TryFormat(bare.Groups[1].Value, bare.Groups[2].Value, out yearMonth);

        return false;
    }

    private static bool TryFormat(string year, string month, out string yearMonth)
    {
        yearMonth = string.Empty;
        if (!int.TryParse(year, NumberStyles.None, CultureInfo.InvariantCulture, out var y)
            || !int.TryParse(month, NumberStyles.None, CultureInfo.InvariantCulture, out var m)
            || m is < 1 or > 12)
        {
            return false;
        }

        yearMonth = $"{y:D4}{m:D2}";
        return true;
    }
}
