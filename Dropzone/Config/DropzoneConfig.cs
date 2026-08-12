using Dropzone.Models;

namespace Dropzone.Config;

/// <summary>
/// Main configuration container for Dropzone
/// </summary>
public class DropzoneConfig
{
    public List<JobConfig> Jobs { get; set; } = new();
}

