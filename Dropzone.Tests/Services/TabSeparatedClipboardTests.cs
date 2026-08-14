using Dropzone.Services;
using FluentAssertions;

namespace Dropzone.Tests.Services;

public class TabSeparatedClipboardTests
{
    [Fact]
    public void Format_WithEmptyRows_ShouldReturnEmptyString()
    {
        TabSeparatedClipboard.Format(Array.Empty<IReadOnlyList<string?>>())
            .Should().BeEmpty();
    }

    [Fact]
    public void Format_ShouldUseTabsBetweenCellsAndCrlfBetweenRows()
    {
        var rows = new List<IReadOnlyList<string?>>
        {
            new string?[] { "5420", "", "10200", "738", "", "", "", "", "144,21", "John Munthe" },
            new string?[] { "P.20257601", "", "", "738", "", "", "5420", "", "7097,97", "John Munthe" }
        };

        var text = TabSeparatedClipboard.Format(rows);

        text.Should().Be(
            "5420\t\t10200\t738\t\t\t\t\t144,21\tJohn Munthe\r\n" +
            "P.20257601\t\t\t738\t\t\t5420\t\t7097,97\tJohn Munthe\r\n");
    }

    [Fact]
    public void Format_ShouldTreatNullCellsAsEmpty()
    {
        var rows = new List<IReadOnlyList<string?>>
        {
            new string?[] { "5420", null, "10200" }
        };

        TabSeparatedClipboard.Format(rows).Should().Be("5420\t\t10200\r\n");
    }
}
