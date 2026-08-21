using Dropzone.Services;
using FluentAssertions;

namespace Dropzone.Tests.Services;

public class InputFileInspectorTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "DropzoneTests", Guid.NewGuid().ToString());

    public InputFileInspectorTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void DescribeIfNotUsablePdf_WithPdfHeader_ShouldReturnNull()
    {
        var path = Write("%PDF-1.7\n1 0 obj\n<<>>\nendobj\n");

        InputFileInspector.DescribeIfNotUsablePdf(path).Should().BeNull();
    }

    [Fact]
    public void DescribeIfNotUsablePdf_WithPdfHeaderAfterJunk_ShouldReturnNull()
    {
        var path = Write("xxxx%PDF-1.4\n");

        InputFileInspector.DescribeIfNotUsablePdf(path).Should().BeNull();
    }

    [Fact]
    public void DescribeIfNotUsablePdf_WithHtmlLoginPage_ShouldExplain()
    {
        var path = Write("<!DOCTYPE html><html><body>Please log in</body></html>");

        var message = InputFileInspector.DescribeIfNotUsablePdf(path);

        message.Should().Contain("web page");
        message.Should().Contain("login");
    }

    [Fact]
    public void DescribeIfNotUsablePdf_WithRandomBytes_ShouldReject()
    {
        var path = Write("This is a plain text invoice, not a PDF.");

        var message = InputFileInspector.DescribeIfNotUsablePdf(path);

        message.Should().Contain("%PDF");
    }

    [Fact]
    public void DescribeIfNotUsablePdf_WithEmptyFile_ShouldReject()
    {
        var path = Path.Combine(_directory, "empty.pdf");
        File.WriteAllBytes(path, []);

        InputFileInspector.DescribeIfNotUsablePdf(path).Should().Contain("empty");
    }

    [Fact]
    public void DescribeIfNotUsablePdf_WithMissingFile_ShouldSayNotFound()
    {
        var path = Path.Combine(_directory, "missing.pdf");

        InputFileInspector.DescribeIfNotUsablePdf(path).Should().Contain("not found");
    }

    private string Write(string content)
    {
        var path = Path.Combine(_directory, $"{Guid.NewGuid()}.pdf");
        File.WriteAllText(path, content);
        return path;
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, true);
        }

        GC.SuppressFinalize(this);
    }
}
