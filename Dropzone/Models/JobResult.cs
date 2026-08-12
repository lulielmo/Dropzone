namespace Dropzone.Models;

/// <summary>
/// Standard result object returned from handlers
/// </summary>
public class JobResult
{
    public List<RowModel> Rows { get; set; } = new();
    public string Comment { get; set; } = string.Empty;
    public string? OutputFile { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

