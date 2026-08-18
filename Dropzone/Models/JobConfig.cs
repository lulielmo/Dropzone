using System.Text.RegularExpressions;

namespace Dropzone.Models;

/// <summary>
/// Configuration for a job type - defines how to match and handle specific cases
/// </summary>
public class JobConfig
{
    public string Name { get; set; } = string.Empty;
    public string? UrlRegex { get; set; }
    public string? FileNameRegex { get; set; }
    public string? FileExtension { get; set; }
    public string? DomainName { get; set; }
    public string? TextRegex { get; set; }
    public string HandlerType { get; set; } = string.Empty;
    public string ViewType { get; set; } = string.Empty;
    public Dictionary<string, string>? HandlerConfig { get; set; }

    /// <summary>
    /// Checks if this config matches the given URL, file path, or dropped text.
    /// </summary>
    public bool Matches(string? url, string? filePath, string? text = null)
    {
        if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(TextRegex)
            && Regex.IsMatch(text, TextRegex, RegexOptions.IgnoreCase))
        {
            return true;
        }
        if (!string.IsNullOrEmpty(url))
        {
            if (!string.IsNullOrEmpty(UrlRegex) && Regex.IsMatch(url, UrlRegex))
                return true;

            if (!string.IsNullOrEmpty(DomainName) && url.Contains(DomainName, StringComparison.OrdinalIgnoreCase))
                return true;

            if (MatchesFileName(GetFileNameFromUrl(url)))
                return true;

            // Check file extension in URL as well
            if (!string.IsNullOrEmpty(FileExtension))
            {
                try
                {
                    var uri = new Uri(url);
                    var path = uri.AbsolutePath;
                    var ext = Path.GetExtension(path).TrimStart('.');
                    if (ext.Equals(FileExtension, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                catch
                {
                    // If URL parsing fails, try extracting extension from the string directly
                    var ext = Path.GetExtension(url).TrimStart('.');
                    if (ext.Equals(FileExtension, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
        }

        if (!string.IsNullOrEmpty(filePath))
        {
            if (MatchesFileName(Path.GetFileName(filePath)))
                return true;

            if (!string.IsNullOrEmpty(FileExtension))
            {
                var ext = Path.GetExtension(filePath).TrimStart('.');
                if (ext.Equals(FileExtension, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return false;
    }

    private bool MatchesFileName(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(FileNameRegex))
            return false;

        return Regex.IsMatch(fileName, FileNameRegex, RegexOptions.IgnoreCase);
    }

    private static string? GetFileNameFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var name = Path.GetFileName(uri.AbsolutePath);
            return string.IsNullOrEmpty(name) ? null : name;
        }
        catch
        {
            var name = Path.GetFileName(url);
            return string.IsNullOrEmpty(name) ? null : name;
        }
    }
}

