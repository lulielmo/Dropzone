using Dropzone.Forms;
using Dropzone.Models;
using FluentAssertions;

namespace Dropzone.Tests.Forms;

public class MainFormWindowSizeTests
{
    [Fact]
    public void Constructor_ShouldUseCompactIdleSize()
    {
        RunSta(() =>
        {
            using var form = new MainForm();

            form.ClientSize.Should().Be(MainForm.IdleClientSize);
            form.MinimumSize.Should().Be(MainForm.IdleMinimumSize);
        });
    }

    [Fact]
    public void ShowResultView_ShouldGrowToResultSize()
    {
        RunSta(() =>
        {
            using var form = new MainForm();

            form.ShowResultView("GridAndCommentView", CreateResult());

            form.ClientSize.Should().Be(MainForm.ResultClientSize);
            form.MinimumSize.Should().Be(MainForm.ResultMinimumSize);
        });
    }

    [Fact]
    public void ShowProcessingView_ShouldStayAtIdleSize()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            form.ShowResultView("GridAndCommentView", CreateResult());

            form.ShowProcessingView();

            form.ClientSize.Should().Be(MainForm.IdleClientSize);
            form.MinimumSize.Should().Be(MainForm.IdleMinimumSize);
        });
    }

    [Fact]
    public void CompleteJobAndReturnToIdle_ShouldShrinkToIdleSize()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            form.ShowResultView("GridAndCommentView", CreateResult());

            form.CompleteJobAndReturnToIdle();

            form.ClientSize.Should().Be(MainForm.IdleClientSize);
            form.MinimumSize.Should().Be(MainForm.IdleMinimumSize);
        });
    }

    [Fact]
    public void ShowResultView_ShouldKeepTopRightCorner()
    {
        RunSta(() =>
        {
            using var form = new MainForm();
            var workingArea = Screen.FromControl(form).WorkingArea;
            form.Location = new Point(workingArea.Right - form.Width, workingArea.Top);
            var expectedRight = form.Right;
            var expectedTop = form.Top;

            form.ShowResultView("GridAndCommentView", CreateResult());

            form.Right.Should().Be(expectedRight);
            form.Top.Should().Be(expectedTop);
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
