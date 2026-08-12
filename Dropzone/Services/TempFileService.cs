namespace Dropzone.Services;

/// <summary>
/// Service for managing temporary files
/// </summary>
public class TempFileService
{
    private readonly string _tempDirectory;

    public TempFileService()
    {
        _tempDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Dropzone",
            "temp"
        );

        // Ensure temp directory exists
        if (!Directory.Exists(_tempDirectory))
        {
            Directory.CreateDirectory(_tempDirectory);
        }
    }

    public string GetTempFilePath(string? originalFileName = null)
    {
        var fileName = originalFileName ?? $"dropzone_{Guid.NewGuid()}.tmp";
        return Path.Combine(_tempDirectory, fileName);
    }

    public void CleanupFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw - cleanup failures shouldn't break the flow
            System.Diagnostics.Debug.WriteLine($"Failed to cleanup temp file {filePath}: {ex.Message}");
        }
    }

    public void CleanupOldFiles(TimeSpan maxAge)
    {
        try
        {
            if (!Directory.Exists(_tempDirectory))
                return;

            var cutoffTime = DateTime.Now - maxAge;
            var files = Directory.GetFiles(_tempDirectory);

            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoffTime)
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    // Ignore individual file cleanup failures
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to cleanup old temp files: {ex.Message}");
        }
    }
}

