using Dropzone.Models;

namespace Dropzone.Forms;

/// <summary>
/// Modal dialog for choosing among multiple matching jobs.
/// </summary>
public class JobSelectionForm : Form
{
    private readonly ListBox _jobListBox;
    private readonly Button _okButton;
    private readonly Button _cancelButton;

    public JobConfig? SelectedJob { get; private set; }

    public JobSelectionForm(IReadOnlyList<JobConfig> jobs)
    {
        if (jobs == null || jobs.Count == 0)
        {
            throw new ArgumentException("At least one job is required.", nameof(jobs));
        }

        Text = "Dropzone";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        TopMost = true;
        ClientSize = new Size(420, 260);
        Padding = new Padding(12);

        var promptLabel = new Label
        {
            Text = "Flera jobb matchar. Välj vilket som ska köras:",
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 36,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _jobListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            DisplayMember = nameof(JobConfig.Name),
            IntegralHeight = false
        };
        foreach (var job in jobs)
        {
            _jobListBox.Items.Add(job);
        }
        _jobListBox.SelectedIndex = 0;
        _jobListBox.DoubleClick += (_, _) => AcceptIfSelected();

        _okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.None,
            Size = new Size(88, 28),
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom
        };
        _okButton.Click += (_, _) => AcceptIfSelected();

        _cancelButton = new Button
        {
            Text = "Avbryt",
            DialogResult = DialogResult.Cancel,
            Size = new Size(88, 28),
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom
        };

        var buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 40
        };
        buttonPanel.Resize += (_, _) => LayoutButtons(buttonPanel);
        buttonPanel.Controls.Add(_okButton);
        buttonPanel.Controls.Add(_cancelButton);
        LayoutButtons(buttonPanel);

        Controls.Add(_jobListBox);
        Controls.Add(buttonPanel);
        Controls.Add(promptLabel);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }

    private void LayoutButtons(Panel buttonPanel)
    {
        const int margin = 0;
        const int gap = 8;
        _cancelButton.Location = new Point(
            buttonPanel.ClientSize.Width - _cancelButton.Width - margin,
            (buttonPanel.ClientSize.Height - _cancelButton.Height) / 2);
        _okButton.Location = new Point(
            _cancelButton.Left - gap - _okButton.Width,
            _cancelButton.Top);
    }

    private void AcceptIfSelected()
    {
        if (_jobListBox.SelectedItem is not JobConfig job)
        {
            return;
        }

        SelectedJob = job;
        DialogResult = DialogResult.OK;
        Close();
    }
}
