using System.Collections.Generic;
using ToeRunner.Model;

namespace ToeRunner.Setup;

/// <summary>
/// Factory class for creating ToeJob instances from RunConfig objects.
/// </summary>
public static class ToeJobFactory
{
    /// <summary>
    /// Creates a list of ToeJob objects from a list of RunConfig objects.
    /// For each RunConfig, creates multiple ToeJobs based on the RunCount and TinyToeConfigPaths.
    /// </summary>
    /// <param name="runConfigs">The list of RunConfig objects to process.</param>
    /// <returns>A list of ToeJob objects.</returns>
    public static List<ToeJob> CreateToeJobs(List<RunConfig> runConfigs)
    {
        var toeJobs = new List<ToeJob>();
        
        foreach (var runConfig in runConfigs)
        {
            for (int count = 1; count <= runConfig.RunCount; count++)
            {
                foreach (var tinyToeConfigPath in runConfig.TinyToeConfigPaths)
                {
                    var toeJob = new ToeJob
                    {
                        RunName = runConfig.Name,
                        Candlestick = runConfig.Candlestick,
                        Name = $"{SanitizePathName(runConfig.Name)}_{count}",
                        BigToeEnvironmentConfigPath = runConfig.BigToeEnvironmentConfigPath,
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
