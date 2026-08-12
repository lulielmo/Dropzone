using Dropzone.Models;

namespace Dropzone.Handlers;

/// <summary>
/// Interface for job handlers that process specific file types or URLs
/// </summary>
public interface IJobHandler
{
    Task<JobResult> ProcessAsync(string inputPath, Dictionary<string, string>? config, CancellationToken cancellationToken = default);
}

