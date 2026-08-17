namespace Dropzone.Models;

/// <summary>
/// A structured diagnostic from a job script (validation failure, warning, or note).
/// Separate from <see cref="JobResult.Comment"/>, which is Medius paste text.
/// </summary>
public class DiagnosticMessage
{
    public DiagnosticLevel Level { get; set; } = DiagnosticLevel.Warning;

    public string Text { get; set; } = string.Empty;

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return LevelLabel(Level);

        return $"{LevelLabel(Level)}  {Text}";
    }

    internal static string LevelLabel(DiagnosticLevel level) => level switch
    {
        DiagnosticLevel.Error => "ERROR",
        DiagnosticLevel.Info => "INFO",
        _ => "WARNING"
    };

    internal static DiagnosticLevel ParseLevel(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DiagnosticLevel.Warning;

        return value.Trim().ToLowerInvariant() switch
        {
            "error" or "err" => DiagnosticLevel.Error,
            "info" or "information" or "note" => DiagnosticLevel.Info,
            "warning" or "warn" => DiagnosticLevel.Warning,
            _ => DiagnosticLevel.Warning
        };
    }
}
