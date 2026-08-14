using Dropzone.Models;

namespace Dropzone.Views;

/// <summary>
/// View displaying a data grid above and a comment text area below
/// </summary>
public partial class GridAndCommentView : UserControl
{
    public GridAndCommentView()
    {
        InitializeComponent();
    }

    public void SetData(JobResult result)
    {
        // Clear existing data
        dataGridView.Rows.Clear();
        commentTextBox.Text = string.Empty;

        // Populate grid with rows (Medius Excel column order A–J)
        foreach (var row in result.Rows)
        {
            dataGridView.Rows.Add(
                row.KonProj,
                row.Empty1 ?? string.Empty,
                row.RG,
                row.Aktivitet,
                row.ProjAkt ?? string.Empty,
                row.Ean ?? string.Empty,
                row.ProjKat ?? string.Empty,
                row.Empty2 ?? string.Empty,
                row.Netto ?? string.Empty,
                row.GodkantAv ?? string.Empty
            );
        }

        // WinForms TextBox needs CRLF; JSON/Python comments typically use LF only.
        commentTextBox.Text = NormalizeNewLines(result.Comment);

        // Select first row if available
        if (dataGridView.Rows.Count > 0)
        {
            dataGridView.Rows[0].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
        }
    }

    private static string NormalizeNewLines(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.ReplaceLineEndings("\r\n");
    }

    private void dataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        // Format cells as needed
    }

    private void dataGridView_SelectionChanged(object? sender, EventArgs e)
    {
        // Handle selection changes if needed
    }
}
