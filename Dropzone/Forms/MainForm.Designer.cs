namespace Dropzone.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private Panel contentPanel;
    private LinkLabel configurationLinkLabel;
    private LinkLabel doneLinkLabel;
    private Panel navigationPanel;

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
        navigationPanel = new Panel();
        configurationLinkLabel = new LinkLabel();
        doneLinkLabel = new LinkLabel();
        contentPanel = new Panel();
        navigationPanel.SuspendLayout();
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
        doneLinkLabel.TabStop = true;
        doneLinkLabel.Text = "Done";
        doneLinkLabel.LinkClicked += doneLinkLabel_LinkClicked;

        // Content Panel
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Name = "contentPanel";
        contentPanel.Padding = new Padding(0);

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

        navigationPanel.ResumeLayout(false);
        navigationPanel.PerformLayout();
        ResumeLayout(false);
    }
}

