using Dropzone.Models;
using FluentAssertions;

namespace Dropzone.Tests.Models;

public class DiagnosticMessageTests
{
    [Fact]
    public void Constructor_ShouldDefaultToWarningAndEmptyText()
    {
        var message = new DiagnosticMessage();

        message.Level.Should().Be(DiagnosticLevel.Warning);
        message.Text.Should().BeEmpty();
    }

    [Theory]
    [InlineData("error", DiagnosticLevel.Error)]
    [InlineData("ERR", DiagnosticLevel.Error)]
    [InlineData("warning", DiagnosticLevel.Warning)]
    [InlineData("warn", DiagnosticLevel.Warning)]
    [InlineData("info", DiagnosticLevel.Info)]
    [InlineData("NOTE", DiagnosticLevel.Info)]
    [InlineData("unknown", DiagnosticLevel.Warning)]
    [InlineData(null, DiagnosticLevel.Warning)]
    [InlineData("  ", DiagnosticLevel.Warning)]
    public void ParseLevel_ShouldMapKnownAliases(string? value, DiagnosticLevel expected)
    {
        DiagnosticMessage.ParseLevel(value).Should().Be(expected);
    }

    [Fact]
    public void ToString_ShouldPrefixLevelLabel()
    {
        var message = new DiagnosticMessage
        {
            Level = DiagnosticLevel.Error,
            Text = "Totalsumman stämmer inte"
        };

        message.ToString().Should().Be("ERROR  Totalsumman stämmer inte");
    }
}
