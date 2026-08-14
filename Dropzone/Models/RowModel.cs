namespace Dropzone.Models;

/// <summary>
/// Represents a single accounting row in Medius paste order (Excel columns A–J).
/// </summary>
public class RowModel
{
    /// <summary>Column A: Kon/Proj</summary>
    public string KonProj { get; set; } = string.Empty;

    /// <summary>Column B: empty spacer</summary>
    public string? Empty1 { get; set; }

    /// <summary>Column C: RG</summary>
    public string RG { get; set; } = string.Empty;

    /// <summary>Column D: Aktivitet</summary>
    public string Aktivitet { get; set; } = string.Empty;

    /// <summary>Column E: ProjAkt</summary>
    public string? ProjAkt { get; set; }

    /// <summary>Column F: EAN</summary>
    public string? Ean { get; set; }

    /// <summary>Column G: ProjKat</summary>
    public string? ProjKat { get; set; }

    /// <summary>Column H: empty spacer</summary>
    public string? Empty2 { get; set; }

    /// <summary>Column I: Netto (typically Swedish decimal comma)</summary>
    public string? Netto { get; set; }

    /// <summary>Column J: Godkänt av</summary>
    public string? GodkantAv { get; set; }
}
