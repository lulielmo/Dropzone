using Dropzone.Models;

namespace Dropzone.Views;

/// <summary>
/// Result view that can bind a <see cref="JobResult"/>.
/// Registered in <c>MainForm</c> by <c>viewType</c>, same pattern as handlers.
/// </summary>
public interface IJobResultView
{
    void SetData(JobResult result);
}
