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
