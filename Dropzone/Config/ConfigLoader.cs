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

    public JobConfig? FindMatchingJob(string? url, string? filePath)
    {
        var config = Load();
        
        foreach (var job in config.Jobs)
        {
            if (job.Matches(url, filePath))
            {
                return job;
            }
        }

        return null;
    }
}

