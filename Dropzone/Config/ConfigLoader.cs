using System.Text.Json;
using Dropzone.Models;

namespace Dropzone.Config;

/// <summary>
/// Service for loading Dropzone configuration from JSON file
/// </summary>
public class ConfigLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly string _configPath;

    public string ConfigPath => _configPath;

    public ConfigLoader(string? configPath = null)
    {
        _configPath = configPath ?? ResolveDefaultConfigPath(AppDomain.CurrentDomain.BaseDirectory);
    }

    /// <summary>
    /// Prefers <c>Config/dropzone.config.json</c> next to <c>Dropzone.csproj</c> when running from a build output folder.
    /// Otherwise uses the copy under the application base directory.
    /// </summary>
    internal static string ResolveDefaultConfigPath(string baseDirectory)
    {
        var outputPath = Path.Combine(baseDirectory, "Config", "dropzone.config.json");
        var projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));
        var projectFile = Path.Combine(projectDirectory, "Dropzone.csproj");
        var projectConfig = Path.Combine(projectDirectory, "Config", "dropzone.config.json");
        if (File.Exists(projectFile) && File.Exists(projectConfig))
        {
            return projectConfig;
        }

        return outputPath;
    }

    public DropzoneConfig Load()
    {
        if (!File.Exists(_configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {_configPath}");
        }

        var jsonContent = File.ReadAllText(_configPath);
        var config = JsonSerializer.Deserialize<DropzoneConfig>(jsonContent, SerializerOptions);

        if (config == null)
        {
            throw new InvalidOperationException("Failed to deserialize configuration file");
        }

        return config;
    }

    public JobConfig? FindMatchingJob(string? url, string? filePath, string? text = null)
    {
        var jobs = FindMatchingJobs(url, filePath, text);
        return jobs.Count > 0 ? jobs[0] : null;
    }

    /// <summary>
    /// Returns all jobs that match the given URL, file path, or dropped text, in config order.
    /// </summary>
    public IReadOnlyList<JobConfig> FindMatchingJobs(string? url, string? filePath, string? text = null)
    {
        var config = Load();
        return config.Jobs.Where(job => job.Matches(url, filePath, text)).ToList();
    }
}

