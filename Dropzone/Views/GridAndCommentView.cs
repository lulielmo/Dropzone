using Dropzone.Models;
using Dropzone.Services;

namespace Dropzone.Views;

/// <summary>
/// View displaying script diagnostics (when present), a data grid, and a Medius comment text area.
/// </summary>
public partial class GridAndCommentView : UserControl, IJobResultView
{
    public GridAndCommentView()
    {
        InitializeComponent();
        dataGridView.KeyDown += dataGridView_KeyDown;
    }

    public void SetData(JobResult result)
    {
        dataGridView.Rows.Clear();
        commentTextBox.Text = string.Empty;

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
        copyCommentButton.Enabled = !string.IsNullOrWhiteSpace(commentTextBox.Text);
        copyGridButton.Enabled = dataGridView.Rows.Count > 0;

        ShowDiagnostics(GetDisplayDiagnostics(result));

        if (dataGridView.Rows.Count > 0)
        {
            dataGridView.Focus();
            dataGridView.SelectAll();
        }
    }

    internal bool IsCopyGridEnabled => copyGridButton.Enabled;

    internal bool IsCopyCommentEnabled => copyCommentButton.Enabled;

    internal string CommentText => commentTextBox.Text;

    internal void CopyGridToClipboard()
    {
        if (dataGridView.Rows.Count == 0 || dataGridView.Columns.Count == 0)
        {
            return;
        }

        CopyCellsAsExcelTsv(0, dataGridView.Rows.Count - 1, 0, dataGridView.Columns.Count - 1);
    }

    internal void ClearGridSelection()
    {
        dataGridView.ClearSelection();
    }

    private void copyGridButton_Click(object? sender, EventArgs e)
    {
        CopyGridToClipboard();
    }

    internal void CopyCommentToClipboard()
    {
        if (string.IsNullOrEmpty(commentTextBox.Text))
        {
            return;
        }

        Clipboard.SetText(commentTextBox.Text);
    }

    private void copyCommentButton_Click(object? sender, EventArgs e)
    {
        CopyCommentToClipboard();
    }

    internal bool DiagnosticsVisible => diagnosticsPanel.Visible;

    internal IReadOnlyList<DiagnosticMessage> DisplayedDiagnostics =>
        diagnosticsListBox.Items.Cast<DiagnosticMessage>().ToList();

    internal static List<DiagnosticMessage> GetDisplayDiagnostics(JobResult result)
    {
        var diagnostics = result.Messages?
            .Where(message => !string.IsNullOrWhiteSpace(message.Text))
            .ToList() ?? [];

        if (!result.Success
            && !string.IsNullOrWhiteSpace(result.ErrorMessage)
            && diagnostics.All(message => message.Text != result.ErrorMessage))
        {
            diagnostics.Insert(0, new DiagnosticMessage
            {
                Level = DiagnosticLevel.Error,
                Text = result.ErrorMessage
            });
        }

        return diagnostics;
    }

    private void ShowDiagnostics(List<DiagnosticMessage> messages)
    {
        diagnosticsListBox.Items.Clear();

        if (messages.Count == 0)
        {
            diagnosticsPanel.Visible = false;
            diagnosticsPanel.Height = 0;
            return;
        }

        foreach (var message in messages)
        {
            diagnosticsListBox.Items.Add(message);
        }

        var visibleRows = Math.Clamp(messages.Count, 1, 5);
        var listHeight = visibleRows * diagnosticsListBox.ItemHeight + 4;
        diagnosticsListBox.Height = listHeight;
        diagnosticsPanel.Height = listHeight + diagnosticsPanel.Padding.Vertical;
        diagnosticsPanel.Visible = true;
        diagnosticsPanel.BackColor = BackColorFor(messages.Max(message => message.Level));
    }

    private void diagnosticsListBox_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= diagnosticsListBox.Items.Count)
            return;

        var message = (DiagnosticMessage)diagnosticsListBox.Items[e.Index];
        var backColor = BackColorFor(message.Level);
        var foreColor = ForeColorFor(message.Level);

        using var background = new SolidBrush(backColor);
        e.Graphics.FillRectangle(background, e.Bounds);

        var textBounds = Rectangle.Inflate(e.Bounds, -8, 0);
        TextRenderer.DrawText(
            e.Graphics,
            message.ToString(),
            diagnosticsListBox.Font,
            textBounds,
            foreColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }

    private static Color BackColorFor(DiagnosticLevel level) => level switch
    {
        DiagnosticLevel.Error => Color.FromArgb(255, 235, 238),
        DiagnosticLevel.Info => Color.FromArgb(227, 242, 253),
        _ => Color.FromArgb(255, 243, 224)
    };

    private static Color ForeColorFor(DiagnosticLevel level) => level switch
    {
        DiagnosticLevel.Error => Color.FromArgb(183, 28, 28),
        DiagnosticLevel.Info => Color.FromArgb(13, 71, 161),
        _ => Color.FromArgb(230, 81, 0)
    };

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

        CopyCellsAsExcelTsv(minRow, maxRow, minCol, maxCol);
    }

    private void CopyCellsAsExcelTsv(int minRow, int maxRow, int minCol, int maxCol)
    {
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

        var text = TabSeparatedClipboard.Format(rows);
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        Clipboard.SetText(text);
    }
}
