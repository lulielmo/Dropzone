using System.Text;

namespace Dropzone.Services;

/// <summary>
/// Sniffs a dropped or downloaded file before a Python script runs.
/// File jobs currently expect a PDF; HTML is usually a login page.
/// </summary>
internal static class InputFileInspector
{
    private const int ProbeSize = 1024;

    /// <summary>
    /// Returns a user-facing reason when the file is not a usable PDF; otherwise null.
    /// </summary>
    public static string? DescribeIfNotUsablePdf(string path)
    {
        if (!File.Exists(path))
        {
            return $"Input file not found:\n{path}";
        }

        var probe = ReadStart(path, ProbeSize);
        if (probe.Length == 0)
        {
            return "The file is empty, so it is not a usable PDF.";
        }

        if (ContainsPdfHeader(probe))
        {
            return null;
        }

        if (LooksLikeHtml(probe))
        {
            return
                "The file is a web page, not a PDF. This often means the site returned a login page " +
                "instead of the document.\n\nDrop the downloaded PDF from your browser instead of the URL.";
        }

        return
            "The file is not a usable PDF (no %PDF header).\n\n" +
            "Drop a PDF, or if this came from Medius, use the file from Downloads rather than the page URL.";
    }

    private static byte[] ReadStart(string path, int maxBytes)
    {
        using var stream = File.OpenRead(path);
        var buffer = new byte[maxBytes];
        var read = stream.Read(buffer, 0, buffer.Length);
        if (read == buffer.Length)
        {
            return buffer;
        }

        return buffer.AsSpan(0, read).ToArray();
    }

    private static bool ContainsPdfHeader(byte[] bytes)
    {
        var marker = "%PDF"u8;
        if (bytes.Length < marker.Length)
        {
            return false;
        }

        for (var i = 0; i <= bytes.Length - marker.Length; i++)
        {
            if (bytes.AsSpan(i, marker.Length).SequenceEqual(marker))
            {
                return true;
            }
        }

        return false;
    }

    private static bool LooksLikeHtml(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF', ' ', '\t', '\r', '\n');
        if (text.Length == 0 || text[0] != '<')
        {
            return false;
        }

        return text.Contains("<html", StringComparison.OrdinalIgnoreCase)
            || text.Contains("<!doctype html", StringComparison.OrdinalIgnoreCase)
            || text.Contains("<head", StringComparison.OrdinalIgnoreCase)
            || text.Contains("<body", StringComparison.OrdinalIgnoreCase);
    }
}
