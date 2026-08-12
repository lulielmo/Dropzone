using System.Text.RegularExpressions;

namespace Dropzone.Models;

/// <summary>
/// Configuration for a job type - defines how to match and handle specific cases
/// </summary>
public class JobConfig
{
    public string Name { get; set; } = string.Empty;
    public string? UrlRegex { get; set; }
    public string? FileExtension { get; set; }
    public string? DomainName { get; set; }
    public string HandlerType { get; set; } = string.Empty;
    public string ViewType { get; set; } = string.Empty;
    public Dictionary<string, string>? HandlerConfig { get; set; }

    /// <summary>
    /// Checks if this config matches the given URL or file path
    /// </summary>
    public bool Matches(string? url, string? filePath)
    {
        if (!string.IsNullOrEmpty(url))
        {
            if (!string.IsNullOrEmpty(UrlRegex) && Regex.IsMatch(url, UrlRegex))
                return true;

            if (!string.IsNullOrEmpty(DomainName) && url.Contains(DomainName, StringComparison.OrdinalIgnoreCase))
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
            if (!string.IsNullOrEmpty(FileExtension))
            {
                var ext = Path.GetExtension(filePath).TrimStart('.');
                if (ext.Equals(FileExtension, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return false;
    }
}

