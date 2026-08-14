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
        row.Empty1.Should().BeNull();
        row.ProjAkt.Should().BeNull();
        row.Ean.Should().BeNull();
        row.ProjKat.Should().BeNull();
        row.Empty2.Should().BeNull();
        row.Netto.Should().BeNull();
        row.GodkantAv.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var row = new RowModel
        {
            KonProj = "5420",
            Empty1 = "",
            RG = "10000",
            Aktivitet = "738",
            ProjAkt = "",
            Ean = "",
            ProjKat = "5420",
            Empty2 = "",
            Netto = "144,21",
            GodkantAv = "John Munthe"
        };

        // Assert
        row.KonProj.Should().Be("5420");
        row.RG.Should().Be("10000");
        row.Aktivitet.Should().Be("738");
        row.ProjKat.Should().Be("5420");
        row.Netto.Should().Be("144,21");
        row.GodkantAv.Should().Be("John Munthe");
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
