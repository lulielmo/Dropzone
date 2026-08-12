using Dropzone.Models;
using FluentAssertions;

namespace Dropzone.Tests.Models;

/// <summary>
/// Tests for RowModel class
/// </summary>
public class RowModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithEmptyStrings()
    {
        // Act
        var row = new RowModel();

        // Assert
        row.KonProj.Should().BeEmpty();
        row.RG.Should().BeEmpty();
        row.Aktivitet.Should().BeEmpty();
        row.Empty.Should().BeNull();
        row.ProjKa.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var row = new RowModel
        {
            KonProj = "5420",
            RG = "10000",
            Aktivitet = "738",
            Empty = null,
            ProjKa = "5420"
        };

        // Assert
        row.KonProj.Should().Be("5420");
        row.RG.Should().Be("10000");
        row.Aktivitet.Should().Be("738");
        row.Empty.Should().BeNull();
        row.ProjKa.Should().Be("5420");
    }

    [Fact]
    public void Properties_ShouldHandleEmptyValues()
    {
        // Arrange
        var row = new RowModel
        {
            KonProj = "",
            RG = "",
            Aktivitet = ""
        };

        // Assert
        row.KonProj.Should().BeEmpty();
        row.RG.Should().BeEmpty();
        row.Aktivitet.Should().BeEmpty();
    }
}

