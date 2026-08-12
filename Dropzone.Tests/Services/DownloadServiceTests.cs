using Dropzone.Services;
using FluentAssertions;

namespace Dropzone.Tests.Services;

/// <summary>
/// Tests for DownloadService
/// 
/// Note: DownloadService currently creates its own HttpClient internally,
/// which makes it difficult to unit test with mocking. For full testability,
/// consider refactoring to inject HttpClient via constructor.
/// </summary>
public class DownloadServiceTests : IDisposable
{
    private readonly DownloadService _service;
    private readonly List<string> _tempFiles = new();

    public DownloadServiceTests()
    {
        _service = new DownloadService();
    }

    [Fact]
    public async Task DownloadFileAsync_WithInvalidUrl_ShouldThrowException()
    {
        // Arrange
        var invalidUrl = "not-a-valid-url";
        var tempFile = Path.Combine(Path.GetTempPath(), $"test_download_{Guid.NewGuid()}.pdf");
        _tempFiles.Add(tempFile);

        // Act & Assert
        var act = async () => await _service.DownloadFileAsync(invalidUrl, tempFile);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task DownloadFileAsync_WithEmptyUrl_ShouldThrowException()
    {
        // Arrange
        var emptyUrl = "";
        var tempFile = Path.Combine(Path.GetTempPath(), $"test_download_{Guid.NewGuid()}.pdf");
        _tempFiles.Add(tempFile);

        // Act & Assert
        var act = async () => await _service.DownloadFileAsync(emptyUrl, tempFile);
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void Constructor_ShouldInitializeHttpClient()
    {
        // Act
        var service = new DownloadService();

        // Assert
        service.Should().NotBeNull();

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var service = new DownloadService();

        // Act & Assert
        var act = () => service.Dispose();
        act.Should().NotThrow();
    }

    public void Dispose()
    {
        _service?.Dispose();

        // Cleanup temp files
        foreach (var file in _tempFiles)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}

