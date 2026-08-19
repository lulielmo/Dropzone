namespace Dropzone.Services;

/// <summary>
/// Service for downloading files from URLs
/// </summary>
public class DownloadService : IDisposable
{
    private readonly HttpClient _httpClient;

    public DownloadService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
    }

    public async Task<string> DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fileStream, cancellationToken);

            return destinationPath;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to download file from {url}: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

