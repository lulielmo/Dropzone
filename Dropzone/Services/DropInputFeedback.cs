namespace Dropzone.Services;

/// <summary>
/// User-facing text for drop matching failures.
/// </summary>
internal static class DropInputFeedback
{
    public static string NoMatchingJob(string? url, string? filePath, string? text)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            return
                $"No job matches this file:\n{Path.GetFileName(filePath)}\n\n" +
                "Jobs match on file name, URL, or dropped text. Open Configuration to review the rules.\n" +
                "Medius attachments are usually dropped as files from Downloads, not as the page URL.";
        }

        if (!string.IsNullOrEmpty(url))
        {
            return
                $"No job matches this URL:\n{Truncate(url, 120)}\n\n" +
                "If this is a Medius attachment link, drop the downloaded file instead. " +
                "A URL often downloads a login page rather than a PDF.";
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            return
                "No job matches this text.\n\n" +
                "Azure consumption needs the line AZURECONS and a period " +
                "(e.g. Period 2026-06-01 -- 2026-06-30).";
        }

        return "No job matches this drop. Open Configuration to review matching rules.";
    }

    internal static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var normalized = value.ReplaceLineEndings(" ");
        return normalized.Length <= maxLength
            ? normalized
            : normalized[..maxLength] + "…";
    }
}
