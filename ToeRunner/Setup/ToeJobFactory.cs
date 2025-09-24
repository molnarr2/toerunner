using System.Collections.Generic;
using ToeRunner.Model;

namespace ToeRunner.Setup;

/// <summary>
/// Factory class for creating ToeJob instances from ToeRunnerConfig objects.
/// </summary>
public static class ToeJobFactory
{
    /// <summary>
    /// Creates a list of ToeJob objects from a ToeRunnerConfig object.
    /// Creates multiple ToeJobs based on the PerFileRunCount and TinyToeConfigPaths.
    /// </summary>
    /// <param name="config">The ToeRunnerConfig object to process.</param>
    /// <returns>A list of ToeJob objects.</returns>
    public static List<ToeJob> CreateToeJobs(ToeRunnerConfig config)
    {
        var toeJobs = new List<ToeJob>();
        
        if (config.TinyToeConfigPaths != null)
        {
            foreach (var tinyToeConfigPath in config.TinyToeConfigPaths)
            {
                for (int count = 1; count <= config.PerFileRunCount; count++)
                {
                    var toeJob = new ToeJob
                    {
                        RunName = config.Name,
                        Name = $"{SanitizePathName(config.Name)}_{Path.GetFileNameWithoutExtension(tinyToeConfigPath)}_{count}",
                        BigToeEnvironmentConfigPath = config.BigToeEnvironmentConfigPath,
                        TinyToeConfigPath = tinyToeConfigPath
                    };
                    
                    toeJobs.Add(toeJob);
                }
            }
        }
        
        return toeJobs;
    }

    /// <summary>
    /// Sanitizes a string by replacing invalid Unix path characters with underscores.
    /// </summary>
    /// <param name="name">The string to sanitize.</param>
    /// <returns>A sanitized string safe for use in file paths.</returns>
    private static string SanitizePathName(string name)
    {
        return name
            .Replace('/', '_')
            .Replace(':', '_')
            .Replace('*', '_')
            .Replace('?', '_')
            .Replace('"', '_')
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace('|', '_')
            .Replace(' ', '_');
    }
}
