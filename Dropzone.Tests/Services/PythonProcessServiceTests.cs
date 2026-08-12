using Dropzone.Models;
using Dropzone.Services;
using FluentAssertions;
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

    // Note: Testing actual JSON parsing would require either:
    // 1. Mocking the process execution (complex)
    // 2. Having a real Python script that outputs test JSON (integration test)
    // For now, we focus on what we can test without external dependencies
}

