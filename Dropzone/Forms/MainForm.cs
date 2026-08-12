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
    private UserControl? _currentView;
    private string? _currentTempFile;

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
            ShowProcessingView();

            // Find matching job configuration
            var jobConfig = _configLoader.FindMatchingJob(url, filePath);
            if (jobConfig == null)
            {
                MessageBox.Show($"No handler found for this input.\nURL: {url}\nFile: {filePath}", 
                    "Dropzone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowIdleView();
                return;
            }

            // Download file if URL
            string inputPath = filePath ?? string.Empty;
            if (!string.IsNullOrEmpty(url))
            {
                var tempPath = _tempFileService.GetTempFilePath(Path.GetFileName(new Uri(url).AbsolutePath));
                await _downloadService.DownloadFileAsync(url, tempPath);
                inputPath = tempPath;
                _currentTempFile = tempPath;
            }
            else if (!string.IsNullOrEmpty(filePath))
            {
                // Copy to temp if needed (or use directly)
                inputPath = filePath;
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

            // Show result view
            ShowResultView(jobConfig.ViewType, result);

            // Cleanup temp file after a delay or on close
            if (!string.IsNullOrEmpty(_currentTempFile) && File.Exists(_currentTempFile))
            {
                // Cleanup will happen on next process or form close
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing input: {ex.Message}", "Dropzone Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowIdleView();
        }
    }

    private void ShowIdleView()
    {
        contentPanel.Controls.Clear();
        _currentView = null;
        
        // Create idle view with parachute icon
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
        doneLinkLabel.Enabled = false;
    }

    private void ShowProcessingView()
    {
        contentPanel.Controls.Clear();
        _currentView = null;

        var processingLabel = new Label
        {
            Text = "Bearbetar...",
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12F, FontStyle.Regular),
            ForeColor = Color.Blue
        };

        contentPanel.Controls.Add(processingLabel);
    }

    private void ShowResultView(string viewType, JobResult result)
    {
        contentPanel.Controls.Clear();

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
                _currentView = null;
                return;
        }

        if (_currentView != null)
        {
            _currentView.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(_currentView);
        }

        configurationLinkLabel.Enabled = true;
        doneLinkLabel.Enabled = true;
        
        // Switch to "Done" tab
        SwitchToDoneView();
    }

    private void SwitchToConfigurationView()
    {
        configurationLinkLabel.Font = new Font(configurationLinkLabel.Font, FontStyle.Underline);
        doneLinkLabel.Font = new Font(doneLinkLabel.Font, FontStyle.Regular);
        
        // TODO: Show configuration view
        ShowIdleView();
    }

    private void SwitchToDoneView()
    {
        configurationLinkLabel.Font = new Font(configurationLinkLabel.Font, FontStyle.Regular);
        doneLinkLabel.Font = new Font(doneLinkLabel.Font, FontStyle.Underline);
        
        // Keep current view (result view)
    }

    private void configurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        SwitchToConfigurationView();
    }

    private void doneLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        SwitchToDoneView();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Cleanup temp file
        if (!string.IsNullOrEmpty(_currentTempFile) && File.Exists(_currentTempFile))
        {
            _tempFileService.CleanupFile(_currentTempFile);
        }

        // Cleanup old temp files (older than 24 hours)
        _tempFileService.CleanupOldFiles(TimeSpan.FromHours(24));

        base.OnFormClosing(e);
    }
}

