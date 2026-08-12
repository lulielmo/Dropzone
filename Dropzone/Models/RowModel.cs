namespace Dropzone.Models;

/// <summary>
/// Represents a single row in the accounting grid
/// </summary>
public class RowModel
{
    public string KonProj { get; set; } = string.Empty;
    public string? Empty { get; set; }
    public string RG { get; set; } = string.Empty;
    public string Aktivitet { get; set; } = string.Empty;
    public string? ProjKa { get; set; }
}

