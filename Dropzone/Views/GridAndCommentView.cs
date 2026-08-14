using Dropzone.Models;
using Dropzone.Services;

namespace Dropzone.Views;

/// <summary>
/// View displaying a data grid above and a comment text area below
/// </summary>
public partial class GridAndCommentView : UserControl
{
    public GridAndCommentView()
    {
        InitializeComponent();
        dataGridView.KeyDown += dataGridView_KeyDown;
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

        if (dataGridView.Rows.Count > 0)
        {
            // Ready for Ctrl+C into Medius: all data cells selected, no headers.
            dataGridView.Focus();
            dataGridView.SelectAll();
        }
    }

    private static string NormalizeNewLines(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.ReplaceLineEndings("\r\n");
    }

    private void dataGridView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            dataGridView.SelectAll();
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        if (e.Control && e.KeyCode == Keys.C)
        {
            CopySelectionAsExcelTsv();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    /// <summary>
    /// Copies the bounding rectangle of the current selection as Excel-like TSV (no headers).
    /// </summary>
    private void CopySelectionAsExcelTsv()
    {
        if (dataGridView.GetCellCount(DataGridViewElementStates.Selected) == 0)
            return;

        var selectedCells = dataGridView.SelectedCells.Cast<DataGridViewCell>().ToList();
        var minRow = selectedCells.Min(c => c.RowIndex);
        var maxRow = selectedCells.Max(c => c.RowIndex);
        var minCol = selectedCells.Min(c => c.ColumnIndex);
        var maxCol = selectedCells.Max(c => c.ColumnIndex);

        var rows = new List<IReadOnlyList<string?>>();
        for (var r = minRow; r <= maxRow; r++)
        {
            var cells = new List<string?>();
            for (var c = minCol; c <= maxCol; c++)
            {
                cells.Add(dataGridView[c, r].Value?.ToString() ?? string.Empty);
            }
            rows.Add(cells);
        }

        Clipboard.SetText(TabSeparatedClipboard.Format(rows));
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
