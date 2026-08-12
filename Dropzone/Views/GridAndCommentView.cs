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

        // Populate grid with rows
        foreach (var row in result.Rows)
        {
            dataGridView.Rows.Add(
                row.KonProj,
                row.Empty ?? string.Empty,
                row.RG,
                row.Aktivitet,
                row.ProjKa ?? string.Empty
            );
        }

        // Set comment text
        commentTextBox.Text = result.Comment;

        // Select first row if available
        if (dataGridView.Rows.Count > 0)
        {
            dataGridView.Rows[0].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
        }
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

