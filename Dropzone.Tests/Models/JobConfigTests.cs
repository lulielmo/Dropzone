using Dropzone.Models;
using FluentAssertions;

namespace Dropzone.Tests.Models;

/// <summary>
/// Tests for JobConfig class
/// </summary>
public class JobConfigTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithEmptyStrings()
    {
        // Act
        var config = new JobConfig();

        // Assert
        config.Name.Should().BeEmpty();
        config.UrlRegex.Should().BeNull();
        config.FileNameRegex.Should().BeNull();
        config.FileExtension.Should().BeNull();
        config.DomainName.Should().BeNull();
        config.HandlerType.Should().BeEmpty();
        config.ViewType.Should().BeEmpty();
        config.HandlerConfig.Should().BeNull();
    }

    [Theory]
    [InlineData("https://atea.se/invoice.pdf", null, true)]
    [InlineData("https://example.com/file.pdf", null, false)]
    [InlineData(null, "invoice.pdf", false)]
    public void Matches_ShouldMatchUrlByDomain(string? url, string? filePath, bool expectedMatch)
    {
        // Arrange
        var config = new JobConfig
        {
            DomainName = "atea.se"
        };

        // Act
        var result = config.Matches(url, filePath);

        // Assert
        result.Should().Be(expectedMatch);
    }

    [Theory]
    [InlineData("https://example.com/file.pdf", null, true)]
    [InlineData("https://example.com/file.xlsx", null, false)]
    [InlineData(null, "test.pdf", true)]
    [InlineData(null, "test.xlsx", false)]
    public void Matches_ShouldMatchByFileExtension(string? url, string? filePath, bool expectedMatch)
    {
        // Arrange
        var config = new JobConfig
        {
            FileExtension = "pdf"
        };

        // Act
        var result = config.Matches(url, filePath);

        // Assert
        result.Should().Be(expectedMatch);
    }

    [Theory]
    [InlineData("https://atea.se/invoice/license", null, true)]
    [InlineData("https://atea.se/invoice", null, true)]
    [InlineData("https://atea.se/document", null, false)]
    public void Matches_ShouldMatchByUrlRegex(string? url, string? filePath, bool expectedMatch)
    {
        // Arrange
        var config = new JobConfig
        {
            UrlRegex = @".*atea.*licens.*|.*invoice.*"
        };

        // Act
        var result = config.Matches(url, filePath);

        // Assert
        result.Should().Be(expectedMatch);
    }

    [Theory]
    [InlineData(null, @"C:\Users\jomu\Downloads\einvoicecapture-embedded-attachment-4a1f11ec-0493-49f8-a1c1-0a427b5c1bc0 (2).pdf", true)]
    [InlineData(null, @"C:\Users\jomu\Downloads\EINVOICECAPTURE-EMBEDDED-ATTACHMENT-abc.pdf", true)]
    [InlineData(null, @"C:\Users\jomu\Downloads\other-invoice.pdf", false)]
    [InlineData("https://example.com/files/einvoicecapture-embedded-attachment-xyz.pdf", null, true)]
    [InlineData("https://example.com/files/other.pdf", null, false)]
    public void Matches_ShouldMatchByFileNameRegex(string? url, string? filePath, bool expectedMatch)
    {
        // Arrange
        var config = new JobConfig
        {
            FileNameRegex = "einvoicecapture-embedded-attachment"
        };

        // Act
        var result = config.Matches(url, filePath);

        // Assert
        result.Should().Be(expectedMatch);
    }

    [Fact]
    public void Matches_ShouldReturnFalseWhenNoMatches()
    {
        // Arrange
        var config = new JobConfig();

        // Act
        var result = config.Matches("https://example.com/file.pdf", null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Matches_ShouldBeCaseInsensitive()
    {
        // Arrange
        var config = new JobConfig
        {
            DomainName = "ATEA.SE"
        };

        // Act
        var result = config.Matches("https://atea.se/file.pdf", null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HandlerConfig_ShouldStoreConfiguration()
    {
        // Arrange
        var config = new JobConfig
        {
            HandlerConfig = new Dictionary<string, string>
            {
                { "pythonScript", "C:\\path\\to\\script.py" },
                { "pythonExe", "python" }
            }
        };

        // Assert
        config.HandlerConfig.Should().ContainKey("pythonScript");
        config.HandlerConfig.Should().ContainKey("pythonExe");
        config.HandlerConfig["pythonScript"].Should().Be("C:\\path\\to\\script.py");
    }
}

