using System.Text;

namespace Dropzone.Services;

/// <summary>
/// Builds tab-separated text similar to copying a cell range from Excel.
/// </summary>
public static class TabSeparatedClipboard
{
    /// <summary>
    /// Formats rows as TSV with CRLF line endings. Empty cells become empty fields between tabs.
    /// </summary>
    public static string Format(IReadOnlyList<IReadOnlyList<string?>> rows)
    {
        if (rows.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (var r = 0; r < rows.Count; r++)
        {
            if (r > 0)
                sb.Append("\r\n");

            var row = rows[r];
            for (var c = 0; c < row.Count; c++)
            {
                if (c > 0)
                    sb.Append('\t');
                sb.Append(row[c] ?? string.Empty);
            }
        }

        // Excel's text clipboard format typically ends with a trailing newline.
        sb.Append("\r\n");
        return sb.ToString();
    }
}
