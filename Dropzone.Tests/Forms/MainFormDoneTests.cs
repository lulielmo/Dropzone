using Dropzone.Forms;
using Dropzone.Models;
using Dropzone.Services;
using FluentAssertions;

namespace Dropzone.Tests.Forms;

/// <summary>
/// Tests for the Done action on the main window.
/// </summary>
public class MainFormDoneTests
{
    [Fact]
    public void Constructor_ShouldStartIdleWithDoneDisabled()
    {
        RunSta(() =>
        {
            using var form = new MainForm();

            form.IsDoneEnabled.Should().BeFalse();
            form.IsIdlePromptVisible.Should().BeTrue();
        });
    }

    [Fact]
    public void ShowResultView_ShouldEnableDone()
    {
        RunSta(() =>
        {
            using var form = new MainForm();

            form.ShowResultView("GridAndCommentView", CreateResult());

            form.IsDoneEnabled.Should().BeTrue();
            form.IsIdlePromptVisible.Should().BeFalse();
        });
    }

    [Fact]
    public void ShowProcessingView_ShouldDisableDone()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            form.ShowResultView("GridAndCommentView", CreateResult());

            form.ShowProcessingView();

            form.IsDoneEnabled.Should().BeFalse();
            form.IsIdlePromptVisible.Should().BeFalse();
        });
    }

    [Fact]
    public void CompleteJobAndReturnToIdle_ShouldRestoreIdleDisableDoneAndDeleteOwnedTempFiles()
    {
        RunSta(() =>
        {
            var tempFile = CreateOwnedTempFile();
            try
            {
                using var form = new MainForm();
                form.RegisterOwnedTempFile(tempFile);
                form.ShowResultView("GridAndCommentView", CreateResult());

                form.CompleteJobAndReturnToIdle();

                form.IsDoneEnabled.Should().BeFalse();
                form.IsIdlePromptVisible.Should().BeTrue();
                File.Exists(tempFile).Should().BeFalse();
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        });
    }

    [Fact]
    public void CompleteJobAndReturnToIdle_ShouldNotDeleteUnregisteredFiles()
    {
        RunSta(() =>
        {
            var userFile = Path.Combine(Path.GetTempPath(), $"dropzone_user_file_{Guid.NewGuid()}.txt");
            File.WriteAllText(userFile, "keep me");
            try
            {
                using var form = new MainForm();
                form.ShowResultView("GridAndCommentView", CreateResult());

                form.CompleteJobAndReturnToIdle();

                File.Exists(userFile).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(userFile))
                {
                    File.Delete(userFile);
                }
            }
        });
    }

    private static JobResult CreateResult() => new()
    {
        Comment = "Test comment",
        Rows =
        [
            new RowModel { KonProj = "5420", RG = "10200", Aktivitet = "738" }
        ]
    };

    private static string CreateOwnedTempFile()
    {
        var path = new TempFileService().GetTempFilePath($"done_test_{Guid.NewGuid()}.txt");
        File.WriteAllText(path, "owned");
        return path;
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
