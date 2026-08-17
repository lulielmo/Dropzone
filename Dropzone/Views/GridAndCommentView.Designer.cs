namespace Dropzone.Views;

partial class GridAndCommentView
{
    private System.ComponentModel.IContainer components = null!;
    private DataGridView dataGridView;
    private TextBox commentTextBox;
    private SplitContainer splitContainer;
    private Panel diagnosticsPanel;
    private ListBox diagnosticsListBox;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        splitContainer = new SplitContainer();
        dataGridView = new DataGridView();
        commentTextBox = new TextBox();
        diagnosticsPanel = new Panel();
        diagnosticsListBox = new ListBox();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
        SuspendLayout();

        // SplitContainer
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.SplitterDistance = 350;
        splitContainer.SplitterWidth = 3;
        splitContainer.FixedPanel = FixedPanel.None;

        // DataGridView (top panel)
        dataGridView.AllowUserToAddRows = false;
        dataGridView.AllowUserToDeleteRows = false;
        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView.Dock = DockStyle.Fill;
        dataGridView.MultiSelect = true;
        dataGridView.Name = "dataGridView";
        dataGridView.ReadOnly = true;
        dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
        // Headers are not copied — Medius paste expects data cells only (Excel-like TSV).
        dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        dataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "KonProj", HeaderText = "Kon/Proj", MinimumWidth = 70 },
            new DataGridViewTextBoxColumn { Name = "Empty1", HeaderText = "", MinimumWidth = 24 },
            new DataGridViewTextBoxColumn { Name = "RG", HeaderText = "RG", MinimumWidth = 50 },
            new DataGridViewTextBoxColumn { Name = "Aktivitet", HeaderText = "Aktivitet", MinimumWidth = 60 },
            new DataGridViewTextBoxColumn { Name = "ProjAkt", HeaderText = "ProjAkt", MinimumWidth = 50 },
            new DataGridViewTextBoxColumn { Name = "Ean", HeaderText = "EAN", MinimumWidth = 40 },
            new DataGridViewTextBoxColumn { Name = "ProjKat", HeaderText = "ProjKat", MinimumWidth = 50 },
            new DataGridViewTextBoxColumn { Name = "Empty2", HeaderText = "", MinimumWidth = 24 },
            new DataGridViewTextBoxColumn { Name = "Netto", HeaderText = "Netto", MinimumWidth = 60 },
            new DataGridViewTextBoxColumn { Name = "GodkantAv", HeaderText = "Godkänt av", MinimumWidth = 80 }
        });
        dataGridView.CellFormatting += dataGridView_CellFormatting;
        dataGridView.SelectionChanged += dataGridView_SelectionChanged;

        splitContainer.Panel1.Controls.Add(dataGridView);

        // Comment TextBox (bottom panel)
        commentTextBox.Dock = DockStyle.Fill;
        commentTextBox.Multiline = true;
        commentTextBox.Name = "commentTextBox";
        commentTextBox.ReadOnly = true;
        commentTextBox.ScrollBars = ScrollBars.Both;
        commentTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);

        splitContainer.Panel2.Controls.Add(commentTextBox);

        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);

        ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();

        // Diagnostics panel (top). Hidden until a result contains messages.
        diagnosticsPanel.Dock = DockStyle.Top;
        diagnosticsPanel.Name = "diagnosticsPanel";
        diagnosticsPanel.Padding = new Padding(6);
        diagnosticsPanel.Visible = false;
        diagnosticsPanel.Height = 40;

        diagnosticsListBox.BorderStyle = BorderStyle.None;
        diagnosticsListBox.Dock = DockStyle.Fill;
        diagnosticsListBox.DrawMode = DrawMode.OwnerDrawFixed;
        diagnosticsListBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        diagnosticsListBox.IntegralHeight = false;
        diagnosticsListBox.ItemHeight = 24;
        diagnosticsListBox.Name = "diagnosticsListBox";
        diagnosticsListBox.DrawItem += diagnosticsListBox_DrawItem;
        diagnosticsPanel.Controls.Add(diagnosticsListBox);

        ResumeLayout(false);

        // Dock order: Fill first, then Top so the diagnostics bar sits above the split.
        Controls.Add(splitContainer);
        Controls.Add(diagnosticsPanel);

        components = new System.ComponentModel.Container();
    }
}

