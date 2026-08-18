using Dropzone.Config;
using Dropzone.Models;
using FluentAssertions;
using System.Text.Json;

namespace Dropzone.Tests.Config;

/// <summary>
/// Tests for ConfigLoader
/// </summary>
public class ConfigLoaderTests : IDisposable
{
    private readonly string _testConfigPath;
    private readonly string _testConfigDirectory;

    public ConfigLoaderTests()
    {
        _testConfigDirectory = Path.Combine(Path.GetTempPath(), "DropzoneTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testConfigDirectory);
        _testConfigPath = Path.Combine(_testConfigDirectory, "test.config.json");
    }

    [Fact]
    public void Load_WithValidConfig_ShouldDeserializeCorrectly()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "Test Job",
                  "urlRegex": ".*test.*",
                  "handlerType": "TestHandler",
                  "viewType": "TestView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);

        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var config = loader.Load();

        // Assert
        config.Should().NotBeNull();
        config.Jobs.Should().HaveCount(1);
        config.Jobs[0].Name.Should().Be("Test Job");
        config.Jobs[0].UrlRegex.Should().Be(".*test.*");
        config.Jobs[0].HandlerType.Should().Be("TestHandler");
        config.Jobs[0].ViewType.Should().Be("TestView");
    }

    [Fact]
    public void Load_WithMissingFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testConfigDirectory, "nonexistent.json");
        var loader = new ConfigLoader(nonExistentPath);

        // Act & Assert
        var act = () => loader.Load();
        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void Load_WithInvalidJson_ShouldThrowException()
    {
        // Arrange
        File.WriteAllText(_testConfigPath, "{ invalid json }");
        var loader = new ConfigLoader(_testConfigPath);

        // Act & Assert
        var act = () => loader.Load();
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Load_WithEmptyJobsArray_ShouldReturnEmptyList()
    {
        // Arrange
        var configContent = """
            {
              "jobs": []
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var config = loader.Load();

        // Assert
        config.Jobs.Should().BeEmpty();
    }

    [Fact]
    public void FindMatchingJob_WithMatchingUrl_ShouldReturnJob()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "Atea Job",
                  "domainName": "atea.se",
                  "handlerType": "AteaHandler",
                  "viewType": "GridView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var job = loader.FindMatchingJob("https://atea.se/invoice.pdf", null);

        // Assert
        job.Should().NotBeNull();
        job!.Name.Should().Be("Atea Job");
    }

    [Fact]
    public void FindMatchingJob_WithMatchingFileExtension_ShouldReturnJob()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "PDF Handler",
                  "fileExtension": "pdf",
                  "handlerType": "PdfHandler",
                  "viewType": "GridView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var job = loader.FindMatchingJob(null, "C:\\test\\file.pdf");

        // Assert
        job.Should().NotBeNull();
        job!.Name.Should().Be("PDF Handler");
    }

    [Fact]
    public void FindMatchingJob_WithNoMatch_ShouldReturnNull()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "Atea Job",
                  "domainName": "atea.se",
                  "handlerType": "AteaHandler",
                  "viewType": "GridView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var job = loader.FindMatchingJob("https://example.com/file.pdf", null);

        // Assert
        job.Should().BeNull();
    }

    [Fact]
    public void FindMatchingJob_WithMultipleJobs_ShouldReturnFirstMatch()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "First Job",
                  "domainName": "example.com",
                  "handlerType": "Handler1",
                  "viewType": "View1"
                },
                {
                  "name": "Second Job",
                  "domainName": "example.com",
                  "handlerType": "Handler2",
                  "viewType": "View2"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var job = loader.FindMatchingJob("https://example.com/file.pdf", null);

        // Assert
        job.Should().NotBeNull();
        job!.Name.Should().Be("First Job");
    }

    [Fact]
    public void FindMatchingJobs_WithMultipleMatches_ShouldReturnAllInConfigOrder()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "ACP Job",
                  "domainName": "mediusflow.com",
                  "handlerType": "PythonScriptHandler",
                  "viewType": "GridAndCommentView"
                },
                {
                  "name": "Azure Job",
                  "domainName": "mediusflow.com",
                  "handlerType": "PythonScriptHandler",
                  "viewType": "GridAndCommentView"
                },
                {
                  "name": "Other Job",
                  "domainName": "example.com",
                  "handlerType": "OtherHandler",
                  "viewType": "GridAndCommentView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);
        var url = "https://cloud.mediusflow.com/skekraft/Attachments/DownloadAttachment?metadataHash=abc";

        // Act
        var jobs = loader.FindMatchingJobs(url, null);

        // Assert
        jobs.Should().HaveCount(2);
        jobs.Select(j => j.Name).Should().Equal("ACP Job", "Azure Job");
    }

    [Fact]
    public void FindMatchingJobs_WithNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "Atea Job",
                  "domainName": "atea.se",
                  "handlerType": "AteaHandler",
                  "viewType": "GridView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var jobs = loader.FindMatchingJobs("https://example.com/file.pdf", null);

        // Assert
        jobs.Should().BeEmpty();
    }

    [Fact]
    public void FindMatchingJobs_WithDroppedAzureText_ShouldMatchTextRegexOnly()
    {
        var configContent = """
            {
              "jobs": [
                {
                  "name": "ACP Job",
                  "fileNameRegex": "einvoicecapture-embedded-attachment",
                  "handlerType": "PythonScriptHandler",
                  "viewType": "GridAndCommentView"
                },
                {
                  "name": "Azure Job",
                  "textRegex": "AZURECONS",
                  "handlerType": "PythonScriptHandler",
                  "viewType": "GridAndCommentView"
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);
        var text = "Tillverkarens artikelnummer: AZURECONS\r\nPeriod 2026-06-01 -- 2026-06-30";

        var jobs = loader.FindMatchingJobs(null, null, text);

        jobs.Should().ContainSingle().Which.Name.Should().Be("Azure Job");
    }

    [Fact]
    public void Load_WithHandlerConfig_ShouldDeserializeCorrectly()
    {
        // Arrange
        var configContent = """
            {
              "jobs": [
                {
                  "name": "Test Job",
                  "handlerType": "TestHandler",
                  "viewType": "TestView",
                  "handlerConfig": {
                    "pythonScript": "C:\\path\\to\\script.py",
                    "pythonExe": "python"
                  }
                }
              ]
            }
            """;
        File.WriteAllText(_testConfigPath, configContent);
        var loader = new ConfigLoader(_testConfigPath);

        // Act
        var config = loader.Load();

        // Assert
        config.Jobs[0].HandlerConfig.Should().NotBeNull();
        config.Jobs[0].HandlerConfig!["pythonScript"].Should().Be("C:\\path\\to\\script.py");
        config.Jobs[0].HandlerConfig!["pythonExe"].Should().Be("python");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_testConfigPath))
            {
                File.Delete(_testConfigPath);
            }
            if (Directory.Exists(_testConfigDirectory))
            {
                Directory.Delete(_testConfigDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}

