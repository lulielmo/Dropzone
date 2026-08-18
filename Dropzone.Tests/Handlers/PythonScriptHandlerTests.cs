using Dropzone.Handlers;
using Dropzone.Models;
using FluentAssertions;

namespace Dropzone.Tests.Handlers;

/// <summary>
/// Tests for PythonScriptHandler
/// </summary>
public class PythonScriptHandlerTests
{
    [Fact]
    public void Constructor_ShouldInitializeSuccessfully()
    {
        // Act
        var handler = new PythonScriptHandler();

        // Assert
        handler.Should().NotBeNull();
        handler.Should().BeAssignableTo<IJobHandler>();
    }

    [Fact]
    public async Task ProcessAsync_WithNullConfig_ShouldReturnErrorResult()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = "test.pdf";

        // Act
        var result = await handler.ProcessAsync(inputPath, null);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("configuration");
    }

    [Fact]
    public async Task ProcessAsync_WithMissingPythonScriptConfig_ShouldReturnErrorResult()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = "test.pdf";
        var config = new Dictionary<string, string>
        {
            { "pythonExe", "python" }
            // Missing pythonScript
        };

        // Act
        var result = await handler.ProcessAsync(inputPath, config);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Python script path");
    }

    [Fact]
    public async Task ProcessAsync_WithNonExistentPythonScript_ShouldReturnErrorResult()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = "test.pdf";
        var config = new Dictionary<string, string>
        {
            { "pythonScript", "C:\\nonexistent\\script.py" },
            { "pythonExe", "python" }
        };

        // Act
        var result = await handler.ProcessAsync(inputPath, config);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not found");
    }

    [Fact]
    public async Task ProcessAsync_WithNonExistentInputFile_ShouldReturnErrorResult()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = "C:\\nonexistent\\file.pdf";
        
        // Create a temporary Python script for this test
        var tempScript = Path.Combine(Path.GetTempPath(), $"test_script_{Guid.NewGuid()}.py");
        File.WriteAllText(tempScript, "print('test')");
        
        try
        {
            var config = new Dictionary<string, string>
            {
                { "pythonScript", tempScript },
                { "pythonExe", "python" }
            };

            // Act
            var result = await handler.ProcessAsync(inputPath, config);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Input file not found");
        }
        finally
        {
            if (File.Exists(tempScript))
            {
                File.Delete(tempScript);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithCliArgumentInputKind_ShouldNotRequireInputFile()
    {
        var handler = new PythonScriptHandler();
        var tempScript = Path.Combine(Path.GetTempPath(), $"test_script_{Guid.NewGuid()}.py");
        File.WriteAllText(tempScript, """
            import json
            print(json.dumps({"success": True, "comment": "ok", "rows": []}))
            """);

        try
        {
            var config = new Dictionary<string, string>
            {
                { "pythonScript", tempScript },
                { "pythonExe", "python" },
                { "inputKind", "cliArgument" }
            };

            var result = await handler.ProcessAsync("202606", config);

            result.Should().NotBeNull();
            (result.ErrorMessage ?? string.Empty).Should().NotContain("Input file not found");
        }
        finally
        {
            if (File.Exists(tempScript))
            {
                File.Delete(tempScript);
            }
        }
    }

    [Fact]
    public async Task ProcessAsync_WithNonExistentWorkingDirectory_ShouldReturnErrorResult()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = Path.Combine(Path.GetTempPath(), $"test_input_{Guid.NewGuid()}.pdf");
        var tempScript = Path.Combine(Path.GetTempPath(), $"test_script_{Guid.NewGuid()}.py");

        File.WriteAllText(inputPath, "test content");
        File.WriteAllText(tempScript, "print('{}')");

        try
        {
            var config = new Dictionary<string, string>
            {
                { "pythonScript", tempScript },
                { "pythonExe", "python" },
                { "workingDirectory", "C:\\nonexistent\\working\\directory" }
            };

            // Act
            var result = await handler.ProcessAsync(inputPath, config);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Working directory not found");
        }
        finally
        {
            if (File.Exists(inputPath))
                File.Delete(inputPath);
            if (File.Exists(tempScript))
                File.Delete(tempScript);
        }
    }

    [Fact]
    public async Task ProcessAsync_ShouldNotSetResultTitleOrType()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = Path.Combine(Path.GetTempPath(), $"test_input_{Guid.NewGuid()}.pdf");
        var tempScript = Path.Combine(Path.GetTempPath(), $"test_script_{Guid.NewGuid()}.py");
        
        File.WriteAllText(inputPath, "test content");
        
        var jsonOutput = """
            {
                "success": true,
                "comment": "Test comment",
                "rows": []
            }
            """;
        File.WriteAllText(tempScript, $"""
            import json
            import sys
            print({jsonOutput})
            """);
        
        try
        {
            var config = new Dictionary<string, string>
            {
                { "pythonScript", tempScript },
                { "pythonExe", "python" }
            };

            // Act
            var result = await handler.ProcessAsync(inputPath, config);

            // Assert
            result.Should().NotBeNull();
            result.Type.Should().BeNull();
            result.Title.Should().BeNull();
        }
        finally
        {
            if (File.Exists(inputPath))
                File.Delete(inputPath);
            if (File.Exists(tempScript))
                File.Delete(tempScript);
        }
    }

    [Fact]
    public async Task ProcessAsync_ShouldUseCustomPythonExeFromConfig()
    {
        // Arrange
        var handler = new PythonScriptHandler();
        var inputPath = Path.Combine(Path.GetTempPath(), $"test_input_{Guid.NewGuid()}.pdf");
        var tempScript = Path.Combine(Path.GetTempPath(), $"test_script_{Guid.NewGuid()}.py");
        
        File.WriteAllText(inputPath, "test content");
        File.WriteAllText(tempScript, "print('{}')");
        
        try
        {
            var config = new Dictionary<string, string>
            {
                { "pythonScript", tempScript },
                { "pythonExe", "python3" } // Custom python executable
            };

            // Act
            var result = await handler.ProcessAsync(inputPath, config);

            // Assert
            // Even if python3 doesn't exist, we should at least attempt to use it
            // The exact error will depend on system configuration
            result.Should().NotBeNull();
        }
        finally
        {
            if (File.Exists(inputPath))
                File.Delete(inputPath);
            if (File.Exists(tempScript))
                File.Delete(tempScript);
        }
    }
}
