using Dropzone.Services;
using FluentAssertions;

namespace Dropzone.Tests.Services;

public class DropInputFeedbackTests
{
    [Fact]
    public void NoMatchingJob_WithFile_ShouldNameTheFileAndSuggestDownloads()
    {
        var message = DropInputFeedback.NoMatchingJob(null, @"C:\Users\jomu\Downloads\invoice.pdf", null);

        message.Should().Contain("invoice.pdf");
        message.Should().Contain("Downloads");
        message.Should().NotContain("URL:");
    }

    [Fact]
    public void NoMatchingJob_WithUrl_ShouldSuggestFileDrop()
    {
        var message = DropInputFeedback.NoMatchingJob("https://cloud.mediusflow.com/skekraft/Attachments/foo", null, null);

        message.Should().Contain("https://cloud.mediusflow.com");
        message.Should().Contain("login page");
    }

    [Fact]
    public void NoMatchingJob_WithText_ShouldMentionAzurecons()
    {
        var message = DropInputFeedback.NoMatchingJob(null, null, "some clipboard text");

        message.Should().Contain("AZURECONS");
        message.Should().Contain("Period");
    }

    [Fact]
    public void Truncate_ShouldCollapseNewlinesAndEllipsize()
    {
        DropInputFeedback.Truncate("a\r\nb", 10).Should().Be("a b");
        DropInputFeedback.Truncate(new string('x', 5), 5).Should().Be("xxxxx");
        DropInputFeedback.Truncate(new string('x', 6), 5).Should().Be("xxxxx…");
    }
}
