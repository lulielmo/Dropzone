using Dropzone.Models;
using FluentAssertions;

namespace Dropzone.Tests.Models;

/// <summary>
/// Tests for JobResult class
/// </summary>
public class JobResultTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Act
        var result = new JobResult();

        // Assert
        result.Rows.Should().NotBeNull().And.BeEmpty();
        result.Comment.Should().BeEmpty();
        result.OutputFile.Should().BeNull();
        result.Type.Should().BeNull();
        result.Title.Should().BeNull();
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var rows = new List<RowModel>
        {
            new() { KonProj = "5420", RG = "10000", Aktivitet = "738" }
        };

        var result = new JobResult
        {
            Rows = rows,
            Comment = "Test comment",
            OutputFile = "output.json",
            Type = "AteaInvoice",
            Title = "Test Title",
            Success = true,
            ErrorMessage = null,
            Timestamp = DateTime.Now
        };

        // Assert
        result.Rows.Should().HaveCount(1);
        result.Rows[0].KonProj.Should().Be("5420");
        result.Comment.Should().Be("Test comment");
        result.OutputFile.Should().Be("output.json");
        result.Type.Should().Be("AteaInvoice");
        result.Title.Should().Be("Test Title");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ErrorResult_ShouldSetSuccessToFalse()
    {
        // Arrange
        var result = new JobResult
        {
            Success = false,
            ErrorMessage = "Something went wrong",
            Comment = "Error: Something went wrong"
        };

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Something went wrong");
        result.Comment.Should().Contain("Error");
    }
}

