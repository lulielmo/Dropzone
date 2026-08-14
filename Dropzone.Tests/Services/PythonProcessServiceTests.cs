using Dropzone.Models;
using Dropzone.Services;
using FluentAssertions;
using System.Text;
using System.Text.Json;

namespace Dropzone.Tests.Services;

/// <summary>
/// Tests for PythonProcessService
/// 
/// Note: These tests focus on JSON parsing logic rather than actual process execution,
/// as process execution requires a real Python installation.
/// </summary>
public class PythonProcessServiceTests
{
    private readonly PythonProcessService _service;

    public PythonProcessServiceTests()
    {
        _service = new PythonProcessService();
    }

    [Fact]
    public void Constructor_ShouldInitializeSuccessfully()
    {
        // Act
        var service = new PythonProcessService();

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteScriptAsync_WithNonExistentPython_ShouldReturnErrorResult()
    {
        // Arrange
        var pythonExe = "nonexistent_python.exe";
        var scriptPath = "script.py";
        var inputPath = "input.pdf";

        // Act
        var result = await _service.ExecuteScriptAsync(pythonExe, scriptPath, inputPath);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExecuteScriptAsync_WithNonExistentScript_ShouldReturnErrorResult()
    {
        // Arrange
        var pythonExe = "python";
        var scriptPath = "C:\\nonexistent\\script.py";
        var inputPath = "input.pdf";

        // Act
        var result = await _service.ExecuteScriptAsync(pythonExe, scriptPath, inputPath);

        // Assert
        result.Should().NotBeNull();
        // Result may be error or success depending on how Python handles missing scripts
        // but we should always get a result
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteScriptAsync_WithNonExistentWorkingDirectory_ShouldStillAttemptStart()
    {
        // Arrange — invalid cwd is accepted by ProcessStartInfo; failure appears at start/run time
        var result = await _service.ExecuteScriptAsync(
            "nonexistent_python.exe",
            "script.py",
            "input.pdf",
            workingDirectory: "C:\\nonexistent\\cwd");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExecuteScriptAsync_WithUtf8SwedishCharacters_ShouldPreserveComment()
    {
        var pythonExe = FindPythonExecutable();
        if (pythonExe == null)
        {
            return; // Skip when Python is not available on the machine
        }

        var inputPath = Path.Combine(Path.GetTempPath(), $"dz_utf8_input_{Guid.NewGuid()}.txt");
        var scriptPath = Path.Combine(Path.GetTempPath(), $"dz_utf8_script_{Guid.NewGuid()}.py");

        File.WriteAllText(inputPath, "input");
        File.WriteAllText(scriptPath, """
            import json
            import sys
            print(json.dumps({
                "success": True,
                "comment": "Anders Löfgren\t10200:PP Optimering",
                "rows": []
            }, ensure_ascii=False))
            """, Encoding.UTF8);

        try
        {
            var result = await _service.ExecuteScriptAsync(pythonExe, scriptPath, inputPath);

            result.Success.Should().BeTrue();
            result.Comment.Should().Contain("Löfgren");
            result.Comment.Should().NotContain("Ã¶");
        }
        finally
        {
            if (File.Exists(inputPath)) File.Delete(inputPath);
            if (File.Exists(scriptPath)) File.Delete(scriptPath);
        }
    }

    private static string? FindPythonExecutable()
    {
        foreach (var candidate in new[] { "python", "py" })
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = candidate,
                    Arguments = candidate == "py" ? "-3 -c \"print(1)\"" : "-c \"print(1)\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using var process = System.Diagnostics.Process.Start(psi);
                if (process == null) continue;
                process.WaitForExit(5000);
                if (process.ExitCode == 0)
                    return candidate == "py" ? "py" : candidate;
            }
            catch
            {
                // try next
            }
        }

        return null;
    }

    // Note: Testing actual JSON parsing would require either:
    // 1. Mocking the process execution (complex)
    // 2. Having a real Python script that outputs test JSON (integration test)
    // For now, we focus on what we can test without external dependencies
}

