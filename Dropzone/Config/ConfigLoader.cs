using System.Text.Json;
using Dropzone.Models;

namespace Dropzone.Config;

/// <summary>
/// Service for loading Dropzone configuration from JSON file
/// </summary>
public class ConfigLoader
{
    private readonly string _configPath;

    public ConfigLoader(string? configPath = null)
    {
        _configPath = configPath ?? Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Config",
            "dropzone.config.json"
        );
    }

    public DropzoneConfig Load()
    {
        if (!File.Exists(_configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {_configPath}");
        }

        var jsonContent = File.ReadAllText(_configPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        var config = JsonSerializer.Deserialize<DropzoneConfig>(jsonContent, options);
        
        if (config == null)
        {
            throw new Exception("Failed to deserialize configuration file");
        }

        return config;
    }

    public JobConfig? FindMatchingJob(string? url, string? filePath, string? text = null)
    {
        return FindMatchingJobs(url, filePath, text).FirstOrDefault();
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

