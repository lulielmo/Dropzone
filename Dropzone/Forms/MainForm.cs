using Dropzone.Config;
using Dropzone.Handlers;
using Dropzone.Models;
using Dropzone.Services;
using Dropzone.Views;

namespace Dropzone.Forms;

public partial class MainForm : Form
{
    private readonly ConfigLoader _configLoader;
    private readonly DownloadService _downloadService;
    private readonly TempFileService _tempFileService;
    private readonly Dictionary<string, Type> _handlerTypes;
    private readonly List<string> _ownedTempFiles = new();
    private UserControl? _currentView;

    public MainForm()
    {
        InitializeComponent();
        
        _configLoader = new ConfigLoader();
        _downloadService = new DownloadService();
        _tempFileService = new TempFileService();
        
        // Register handler types
        _handlerTypes = new Dictionary<string, Type>
        {
            { "AteaInvoiceHandler", typeof(AteaInvoiceHandler) }
        };

        // Setup drag and drop
        AllowDrop = true;
        DragEnter += MainForm_DragEnter;
        DragDrop += MainForm_DragDrop;

        // Show idle view initially
        ShowIdleView();
    }

    private void MainForm_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data == null) return;

        if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private async void MainForm_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data == null) return;

        string? url = null;
        string? filePath = null;

        // Check for URL (text)
        if (e.Data.GetDataPresent(DataFormats.Text))
        {
            var text = e.Data.GetData(DataFormats.Text) as string;
            if (!string.IsNullOrEmpty(text) && Uri.TryCreate(text, UriKind.Absolute, out _))
            {
                url = text;
            }
        }

        // Check for file drop
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
            {
                filePath = files[0];
            }
        }

        if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(filePath))
        {
            MessageBox.Show("Invalid drop: Please drop a URL or file.", "Dropzone", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ProcessInput(url, filePath);
    }

    private async Task ProcessInput(string? url, string? filePath)
    {
        try
        {
            var matchingJobs = _configLoader.FindMatchingJobs(url, filePath);
            if (matchingJobs.Count == 0)
            {
                MessageBox.Show($"No handler found for this input.\nURL: {url}\nFile: {filePath}",
                    "Dropzone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowIdleView();
                return;
            }

            var jobConfig = ResolveJobSelection(matchingJobs);
            if (jobConfig == null)
            {
                ShowIdleView();
                return;
            }

            ShowProcessingView();

            // Download file if URL. Dropped local files are used as-is and never deleted.
            string inputPath = filePath ?? string.Empty;
            if (!string.IsNullOrEmpty(url))
            {
                var tempPath = _tempFileService.GetTempFilePath(Path.GetFileName(new Uri(url).AbsolutePath));
                RegisterOwnedTempFile(tempPath);
                await _downloadService.DownloadFileAsync(url, tempPath);
                inputPath = tempPath;
            }

            // Create handler instance
            if (!_handlerTypes.TryGetValue(jobConfig.HandlerType, out var handlerType))
            {
                throw new Exception($"Unknown handler type: {jobConfig.HandlerType}");
            }

            var handler = Activator.CreateInstance(handlerType) as IJobHandler;
            if (handler == null)
            {
                throw new Exception($"Failed to create handler: {jobConfig.HandlerType}");
            }

            // Process with handler
            var result = await handler.ProcessAsync(inputPath, jobConfig.HandlerConfig);

            ShowResultView(jobConfig.ViewType, result);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing input: {ex.Message}", "Dropzone Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowIdleView();
        }
    }

    /// <summary>
    /// Picks the single matching job, or asks the user when several jobs match.
    /// Returns null if the user cancels.
    /// </summary>
    private JobConfig? ResolveJobSelection(IReadOnlyList<JobConfig> matchingJobs)
    {
        if (matchingJobs.Count == 1)
        {
            return matchingJobs[0];
        }

        using var selectionForm = new JobSelectionForm(matchingJobs);
        var result = selectionForm.ShowDialog(this);
        return result == DialogResult.OK ? selectionForm.SelectedJob : null;
    }

    internal void ShowIdleView()
    {
        CleanupOwnedTempFiles();
        ClearContent();

        var idleLabel = new Label
        {
            Text = "Dropzone\n\nSläpp något här",
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Regular),
            ForeColor = Color.Gray
        };

        contentPanel.Controls.Add(idleLabel);

        configurationLinkLabel.Enabled = true;
        SetDoneEnabled(false);
    }

    internal void ShowProcessingView()
    {
        CleanupOwnedTempFiles();
        ClearContent();

        var processingLabel = new Label
        {
            Text = "Bearbetar...",
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12F, FontStyle.Regular),
            ForeColor = Color.Blue
        };

        contentPanel.Controls.Add(processingLabel);

        SetDoneEnabled(false);
    }

    internal void ShowResultView(string viewType, JobResult result)
    {
        ClearContent();

        switch (viewType)
        {
            case "GridAndCommentView":
                _currentView = new GridAndCommentView();
                ((GridAndCommentView)_currentView).SetData(result);
                break;
            default:
                var errorLabel = new Label
                {
                    Text = $"Unknown view type: {viewType}",
                    Dock = DockStyle.Fill
                };
                contentPanel.Controls.Add(errorLabel);
                SetDoneEnabled(true);
                return;
        }

        if (_currentView != null)
        {
            _currentView.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(_currentView);
        }

        configurationLinkLabel.Enabled = true;
        SetDoneEnabled(true);
    }

    private void SwitchToConfigurationView()
    {
        configurationLinkLabel.Font = new Font(configurationLinkLabel.Font, FontStyle.Underline);
        doneLinkLabel.Font = new Font(doneLinkLabel.Font, FontStyle.Regular);

        // TODO: Show configuration view
        ShowIdleView();
    }

    /// <summary>
    /// Clears the current result, deletes Dropzone-owned temp files, and restores the idle prompt.
    /// Original files dropped from disk are not deleted.
    /// </summary>
    internal void CompleteJobAndReturnToIdle()
    {
        ShowIdleView();
    }

    private void SetDoneEnabled(bool enabled)
    {
        doneLinkLabel.Enabled = enabled;
        doneLinkLabel.TabStop = enabled;
    }

    internal bool IsDoneEnabled => doneLinkLabel.Enabled;

    internal bool IsIdlePromptVisible =>
        contentPanel.Controls.OfType<Label>().Any(label =>
            label.Text.Contains("Släpp något här", StringComparison.Ordinal));

    internal void RegisterOwnedTempFile(string path)
    {
        _ownedTempFiles.Add(path);
    }

    private void CleanupOwnedTempFiles()
    {
        foreach (var path in _ownedTempFiles)
        {
            _tempFileService.CleanupFile(path);
        }

        _ownedTempFiles.Clear();
    }

    private void ClearContent()
    {
        var controls = contentPanel.Controls.Cast<Control>().ToList();
        contentPanel.Controls.Clear();
        foreach (var control in controls)
        {
            control.Dispose();
        }

        _currentView = null;
    }

    private void configurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        SwitchToConfigurationView();
    }

    private void doneLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (!doneLinkLabel.Enabled)
        {
            return;
        }

        CompleteJobAndReturnToIdle();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        CleanupOwnedTempFiles();
        _tempFileService.CleanupOldFiles(TimeSpan.FromHours(24));

        base.OnFormClosing(e);
    }
}

