using Dropzone.Services;
using FluentAssertions;
using System.IO;

namespace Dropzone.Tests.Services;

/// <summary>
/// Tests for TempFileService
/// </summary>
public class TempFileServiceTests : IDisposable
{
    private readonly TempFileService _service;
    private readonly List<string> _tempFiles = new();

    public TempFileServiceTests()
    {
        _service = new TempFileService();
    }

    [Fact]
    public void GetTempFilePath_ShouldReturnValidPath()
    {
        // Act
        var path = _service.GetTempFilePath();

        // Assert
        path.Should().NotBeNullOrEmpty();
        Path.IsPathRooted(path).Should().BeTrue();
        Directory.Exists(Path.GetDirectoryName(path)).Should().BeTrue();
    }

    [Fact]
    public void GetTempFilePath_WithFileName_ShouldUseProvidedName()
    {
        // Arrange
        var fileName = "test_file.pdf";

        // Act
        var path = _service.GetTempFilePath(fileName);

        // Assert
        Path.GetFileName(path).Should().Be(fileName);
        Directory.Exists(Path.GetDirectoryName(path)).Should().BeTrue();
    }

    [Fact]
    public void GetTempFilePath_ShouldCreateDifferentPaths()
    {
        // Act
        var path1 = _service.GetTempFilePath();
        var path2 = _service.GetTempFilePath();

        // Assert
        path1.Should().NotBe(path2);
    }

    [Fact]
    public void CleanupFile_ShouldDeleteExistingFile()
    {
        // Arrange
        var tempFile = _service.GetTempFilePath("test_cleanup.txt");
        _tempFiles.Add(tempFile);
        File.WriteAllText(tempFile, "test content");
        File.Exists(tempFile).Should().BeTrue();

        // Act
        TempFileService.CleanupFile(tempFile);

        // Assert
        File.Exists(tempFile).Should().BeFalse();
    }

    [Fact]
    public void CleanupFile_ShouldNotThrowOnNonExistentFile()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent_file.txt");

        // Act & Assert
        var act = () => TempFileService.CleanupFile(nonExistentFile);
        act.Should().NotThrow();
    }

    [Fact]
    public void CleanupOldFiles_ShouldDeleteFilesOlderThanMaxAge()
    {
        // Arrange
        var oldFile = _service.GetTempFilePath("old_file.txt");
        var newFile = _service.GetTempFilePath("new_file.txt");
        _tempFiles.Add(oldFile);
        _tempFiles.Add(newFile);

        File.WriteAllText(oldFile, "old content");
        File.WriteAllText(newFile, "new content");

        // Set old file's last write time to 2 days ago
        File.SetLastWriteTime(oldFile, DateTime.Now.AddDays(-2));

        // Act
        _service.CleanupOldFiles(TimeSpan.FromHours(24));

        // Assert
        File.Exists(oldFile).Should().BeFalse();
        File.Exists(newFile).Should().BeTrue();

        // Cleanup
        TempFileService.CleanupFile(newFile);
    }

    [Fact]
    public void CleanupOldFiles_ShouldNotDeleteRecentFiles()
    {
        // Arrange
        var recentFile = _service.GetTempFilePath("recent_file.txt");
        _tempFiles.Add(recentFile);
        File.WriteAllText(recentFile, "recent content");

        // Act
        _service.CleanupOldFiles(TimeSpan.FromHours(24));

        // Assert
        File.Exists(recentFile).Should().BeTrue();

        // Cleanup
        TempFileService.CleanupFile(recentFile);
    }

    public void Dispose()
    {
        // Cleanup all created temp files
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

