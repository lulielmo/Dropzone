using Dropzone.Forms;
using FluentAssertions;

namespace Dropzone.Tests.Forms;

public class MainFormWindowBehaviorTests
{
    [Fact]
    public void CreateOpenConfigProcessStartInfo_ShouldOpenFileWithShellExecute()
    {
        var path = @"C:\Dropzone\Config\dropzone.config.json";
        var info = MainForm.CreateOpenConfigProcessStartInfo(path);

        info.FileName.Should().Be(path);
        info.UseShellExecute.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldBeAlwaysOnTop()
    {
        RunSta(() =>
        {
            using var form = new MainForm();

            form.TopMost.Should().BeTrue();
            form.IsInTray.Should().BeFalse();
        });
    }

    [Fact]
    public void HideToTray_ShouldHideWindowAndClearAlwaysOnTop()
    {
        RunSta(() =>
        {
            using var form = new MainForm();

            form.HideToTray();

            form.IsInTray.Should().BeTrue();
            form.Visible.Should().BeFalse();
            form.ShowInTaskbar.Should().BeFalse();
            form.TopMost.Should().BeFalse();
        });
    }

    [Fact]
    public void RestoreFromTray_ShouldShowWindowAndRestoreAlwaysOnTop()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            form.HideToTray();

            form.RestoreFromTray();

            form.IsInTray.Should().BeFalse();
            form.Visible.Should().BeTrue();
            form.ShowInTaskbar.Should().BeTrue();
            form.WindowState.Should().Be(FormWindowState.Normal);
            form.TopMost.Should().BeTrue();
        });
    }

    [Fact]
    public void RestoreFromTray_AfterMinimize_ShouldRestoreVisibleNormalWindow()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            form.WindowState = FormWindowState.Minimized;
            form.HideToTray();

            form.WindowState.Should().Be(FormWindowState.Normal);
            form.Visible.Should().BeFalse();

            form.RestoreFromTray();

            form.IsInTray.Should().BeFalse();
            form.Visible.Should().BeTrue();
            form.ShowInTaskbar.Should().BeTrue();
            form.WindowState.Should().Be(FormWindowState.Normal);
            form.TopMost.Should().BeTrue();
        });
    }

    [Fact]
    public void RunModalUi_ShouldDropAlwaysOnTopDuringDialogAndRestoreAfter()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            form.Show();

            var topMostDuringDialog = true;
            form.RunModalUi(() =>
            {
                topMostDuringDialog = form.TopMost;
                return 0;
            });

            topMostDuringDialog.Should().BeFalse();
            form.TopMost.Should().BeTrue();
        });
    }

    private static void RunSta(Action action)
    {
        Exception? caught = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                caught = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (caught is not null)
        {
            throw caught;
        }
    }
}
