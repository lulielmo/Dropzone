using System.Text;
using System.Text.Json;
using Dropzone.Models;

namespace Dropzone.Services;

/// <summary>
/// Service for executing Python scripts and parsing their JSON output
/// </summary>
public class PythonProcessService
{
    public async Task<JobResult> ExecuteScriptAsync(
        string pythonExe,
        string scriptPath,
        string inputFilePath,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default)
    {
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = $"\"{scriptPath}\" \"{inputFilePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            // Python writes UTF-8 JSON; without this, Windows default ANSI decoding causes mojibake (e.g. "LÃ¶fgren").
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        // Prefer UTF-8 from the Python runtime on Windows as well.
        processStartInfo.Environment["PYTHONIOENCODING"] = "utf-8";
        processStartInfo.Environment["PYTHONUTF8"] = "1";

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            processStartInfo.WorkingDirectory = workingDirectory;
        }

        try
        {
            using var process = System.Diagnostics.Process.Start(processStartInfo);
            if (process == null)
            {
                throw new Exception("Failed to start Python process");
            }

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new Exception($"Python script failed with exit code {process.ExitCode}: {error}");
            }

            // Try to parse JSON output
            if (string.IsNullOrWhiteSpace(output))
            {
                throw new Exception("Python script returned no output");
            }

            return ParseJsonOutput(output);
        }
        catch (Exception ex)
        {
            return new JobResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Rows = new List<RowModel>(),
                Comment = $"Error: {ex.Message}"
            };
        }
    }

    private JobResult ParseJsonOutput(string jsonOutput)
    {
        try
        {
            // Try to parse the JSON output
            var jsonDoc = JsonDocument.Parse(jsonOutput);
            var root = jsonDoc.RootElement;

            var result = new JobResult
            {
                Success = root.TryGetProperty("success", out var success) && success.GetBoolean(),
                Comment = root.TryGetProperty("comment", out var comment) ? comment.GetString() ?? string.Empty : string.Empty,
                Rows = new List<RowModel>()
            };

            // Parse rows if present
            if (root.TryGetProperty("rows", out var rowsElement) && rowsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var rowElement in rowsElement.EnumerateArray())
                {
                    var row = new RowModel
                    {
                        KonProj = rowElement.TryGetProperty("konProj", out var kp) ? kp.GetString() ?? string.Empty : string.Empty,
                        RG = rowElement.TryGetProperty("rg", out var rg) ? rg.GetString() ?? string.Empty : string.Empty,
                        Aktivitet = rowElement.TryGetProperty("aktivitet", out var akt) ? akt.GetString() ?? string.Empty : string.Empty,
                        ProjKa = rowElement.TryGetProperty("projKa", out var pk) ? pk.GetString() : null
                    };
                    result.Rows.Add(row);
                }
            }

            return result;
        }
        catch (JsonException ex)
        {
            // If JSON parsing fails, try to treat the entire output as comment
            return new JobResult
            {
                Success = false,
                ErrorMessage = $"Failed to parse JSON output: {ex.Message}",
                Comment = jsonOutput,
                Rows = new List<RowModel>()
            };
        }
    }
}

