namespace Dropzone.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private Panel contentPanel;
    private LinkLabel configurationLinkLabel;
    private LinkLabel doneLinkLabel;
    private Panel navigationPanel;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip trayMenu;
    private ToolStripMenuItem showTrayMenuItem;
    private ToolStripMenuItem exitTrayMenuItem;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        navigationPanel = new Panel();
        configurationLinkLabel = new LinkLabel();
        doneLinkLabel = new LinkLabel();
        contentPanel = new Panel();
        notifyIcon = new NotifyIcon(components);
        trayMenu = new ContextMenuStrip(components);
        showTrayMenuItem = new ToolStripMenuItem();
        exitTrayMenuItem = new ToolStripMenuItem();
        navigationPanel.SuspendLayout();
        trayMenu.SuspendLayout();
        SuspendLayout();

        // Navigation Panel
        navigationPanel.Controls.Add(configurationLinkLabel);
        navigationPanel.Controls.Add(doneLinkLabel);
        navigationPanel.Dock = DockStyle.Top;
        navigationPanel.Height = 30;
        navigationPanel.Padding = new Padding(10, 5, 0, 5);

        // Configuration LinkLabel
        configurationLinkLabel.AutoSize = true;
        configurationLinkLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        configurationLinkLabel.Location = new Point(10, 7);
        configurationLinkLabel.Name = "configurationLinkLabel";
        configurationLinkLabel.Size = new Size(89, 15);
        configurationLinkLabel.TabIndex = 0;
        configurationLinkLabel.TabStop = true;
        configurationLinkLabel.Text = "Configuration";
        configurationLinkLabel.LinkClicked += configurationLinkLabel_LinkClicked;

        // Done LinkLabel
        doneLinkLabel.AutoSize = true;
        doneLinkLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        doneLinkLabel.Location = new Point(110, 7);
        doneLinkLabel.Name = "doneLinkLabel";
        doneLinkLabel.Size = new Size(36, 15);
        doneLinkLabel.TabIndex = 1;
        doneLinkLabel.TabStop = false;
        doneLinkLabel.Enabled = false;
        doneLinkLabel.Text = "Done";
        doneLinkLabel.LinkClicked += doneLinkLabel_LinkClicked;

        // Content Panel
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Name = "contentPanel";
        contentPanel.Padding = new Padding(0);

        // Tray menu
        showTrayMenuItem.Name = "showTrayMenuItem";
        showTrayMenuItem.Text = "Show";
        showTrayMenuItem.Click += showTrayMenuItem_Click;
        exitTrayMenuItem.Name = "exitTrayMenuItem";
        exitTrayMenuItem.Text = "Exit";
        exitTrayMenuItem.Click += exitTrayMenuItem_Click;
        trayMenu.Items.AddRange(new ToolStripItem[] { showTrayMenuItem, exitTrayMenuItem });
        trayMenu.Name = "trayMenu";

        notifyIcon.ContextMenuStrip = trayMenu;
        notifyIcon.Text = "Dropzone";
        notifyIcon.Visible = false;
        notifyIcon.DoubleClick += notifyIcon_DoubleClick;

        // MainForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 700);
        Controls.Add(contentPanel);
        Controls.Add(navigationPanel);
        MinimumSize = new Size(800, 500);
        Name = "MainForm";
        Text = "Dropzone";
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;

        navigationPanel.ResumeLayout(false);
        navigationPanel.PerformLayout();
        trayMenu.ResumeLayout(false);
        ResumeLayout(false);
    }
}

