using Dropzone.Models;
using Dropzone.Services;

namespace Dropzone.Handlers;

/// <summary>
/// Handler for Atea invoice license files
/// </summary>
public class AteaInvoiceHandler : IJobHandler
{
    private readonly PythonProcessService _pythonService;

    public AteaInvoiceHandler()
    {
        _pythonService = new PythonProcessService();
    }

    public async Task<JobResult> ProcessAsync(string inputPath, Dictionary<string, string>? config, CancellationToken cancellationToken = default)
    {
        if (config == null)
        {
            return new JobResult
            {
                Success = false,
                ErrorMessage = "Handler configuration is missing",
                Comment = "Error: Handler configuration is missing"
            };
        }

        var pythonScript = config.GetValueOrDefault("pythonScript", string.Empty);
        var pythonExe = config.GetValueOrDefault("pythonExe", "python");
        var workingDirectory = config.GetValueOrDefault("workingDirectory", string.Empty);

        if (string.IsNullOrEmpty(pythonScript))
        {
            return new JobResult
            {
                Success = false,
                ErrorMessage = "Python script path not configured",
                Comment = "Error: Python script path not configured in handler config"
            };
        }

        if (!File.Exists(pythonScript))
        {
            return new JobResult
            {
                Success = false,
                ErrorMessage = $"Python script not found: {pythonScript}",
                Comment = $"Error: Python script not found at {pythonScript}"
            };
        }

        if (!string.IsNullOrWhiteSpace(workingDirectory) && !Directory.Exists(workingDirectory))
        {
            return new JobResult
            {
                Success = false,
                ErrorMessage = $"Working directory not found: {workingDirectory}",
                Comment = $"Error: Working directory not found at {workingDirectory}"
            };
        }

        var inputKind = config.GetValueOrDefault("inputKind", "file");
        var requiresInputFile = !inputKind.Equals("cliArgument", StringComparison.OrdinalIgnoreCase);
        if (requiresInputFile && !File.Exists(inputPath))
        {
            return new JobResult
            {
                Success = false,
                ErrorMessage = $"Input file not found: {inputPath}",
                Comment = $"Error: Input file not found at {inputPath}"
            };
        }

        var result = await _pythonService.ExecuteScriptAsync(
            pythonExe,
            pythonScript,
            inputPath,
            string.IsNullOrWhiteSpace(workingDirectory) ? null : workingDirectory,
            cancellationToken);
        
        result.Type = "AteaInvoice";
        result.Title = "Atea Invoice License";

        return result;
    }
}

