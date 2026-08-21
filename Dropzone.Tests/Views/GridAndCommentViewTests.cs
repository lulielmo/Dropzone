using Dropzone.Models;
using Dropzone.Views;
using FluentAssertions;

namespace Dropzone.Tests.Views;

public class GridAndCommentViewTests
{
    [Fact]
    public void GetDisplayDiagnostics_ShouldReturnScriptMessages()
    {
        var result = new JobResult
        {
            Success = true,
            Messages =
            [
                new DiagnosticMessage { Level = DiagnosticLevel.Error, Text = "Totalsumman stämmer inte" }
            ]
        };

        var diagnostics = GridAndCommentView.GetDisplayDiagnostics(result);

        diagnostics.Should().ContainSingle()
            .Which.Text.Should().Be("Totalsumman stämmer inte");
    }

    [Fact]
    public void GetDisplayDiagnostics_WhenFailedWithoutMessages_ShouldUseErrorMessage()
    {
        var result = new JobResult
        {
            Success = false,
            ErrorMessage = "Python script failed",
            Comment = "Error: Python script failed"
        };

        var diagnostics = GridAndCommentView.GetDisplayDiagnostics(result);

        diagnostics.Should().ContainSingle();
        diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
        diagnostics[0].Text.Should().Be("Python script failed");
    }

    [Fact]
    public void GetDisplayDiagnostics_ShouldNotDuplicateMatchingErrorMessage()
    {
        var result = new JobResult
        {
            Success = false,
            ErrorMessage = "Python script failed",
            Messages =
            [
                new DiagnosticMessage { Level = DiagnosticLevel.Error, Text = "Python script failed" }
            ]
        };

        GridAndCommentView.GetDisplayDiagnostics(result).Should().ContainSingle();
    }

    [Fact]
    public void SetData_ShouldShowDiagnosticsSeparatelyFromComment()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            var result = new JobResult
            {
                Success = true,
                Comment = "Medius comment",
                Messages =
                [
                    new DiagnosticMessage { Level = DiagnosticLevel.Error, Text = "Totalsumman stämmer inte" },
                    new DiagnosticMessage { Level = DiagnosticLevel.Warning, Text = "Rad 2 saknar aktivitet" }
                ]
            };

            view.SetData(result);

            view.DiagnosticsVisible.Should().BeTrue();
            view.DisplayedDiagnostics.Should().HaveCount(2);
            view.DisplayedDiagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
        });
    }

    [Fact]
    public void SetData_WithoutMessages_ShouldHideDiagnosticsPanel()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult
            {
                Success = true,
                Comment = "Medius comment"
            });

            view.DiagnosticsVisible.Should().BeFalse();
            view.DisplayedDiagnostics.Should().BeEmpty();
        });
    }

    [Fact]
    public void SetData_WithComment_ShouldEnableCopyComment()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult
            {
                Success = true,
                Comment = "Medius comment\nline 2"
            });

            view.IsCopyCommentEnabled.Should().BeTrue();
            view.CommentText.Should().Contain("Medius comment");
        });
    }

    [Fact]
    public void SetData_WithoutComment_ShouldDisableCopyComment()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult { Success = true, Comment = "  " });

            view.IsCopyCommentEnabled.Should().BeFalse();
        });
    }

    [Fact]
    public void CopyCommentToClipboard_ShouldCopyNormalizedComment()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult
            {
                Success = true,
                Comment = "Line 1\nLine 2"
            });

            view.CopyCommentToClipboard();

            Clipboard.GetText().Should().Be("Line 1\r\nLine 2");
        });
    }

    [Fact]
    public void SetData_WithRows_ShouldEnableCopyGrid()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult
            {
                Success = true,
                Rows = [new RowModel { KonProj = "5420", RG = "10200", Aktivitet = "738" }]
            });

            view.IsCopyGridEnabled.Should().BeTrue();
        });
    }

    [Fact]
    public void SetData_WithoutRows_ShouldDisableCopyGrid()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult { Success = true });

            view.IsCopyGridEnabled.Should().BeFalse();
        });
    }

    [Fact]
    public void CopyGridToClipboard_ShouldCopyAllRowsWithoutSelection()
    {
        RunSta(() =>
        {
            using var view = new GridAndCommentView();
            view.SetData(new JobResult
            {
                Success = true,
                Rows =
                [
                    new RowModel { KonProj = "5420", RG = "10200", Aktivitet = "738", Netto = "144,21", GodkantAv = "John Munthe" },
                    new RowModel { KonProj = "P.20257601", Aktivitet = "738", ProjKat = "5420", Netto = "7097,97", GodkantAv = "John Munthe" }
                ]
            });
            view.ClearGridSelection();

            view.CopyGridToClipboard();

            Clipboard.GetText().Should().Be(
                "5420\t\t10200\t738\t\t\t\t\t144,21\tJohn Munthe\r\n" +
                "P.20257601\t\t\t738\t\t\t5420\t\t7097,97\tJohn Munthe\r\n");
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
