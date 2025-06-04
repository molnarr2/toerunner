using ToeRunner.Model;
using ToeRunner.Plugin;

namespace ToeRunner.ToeRun;

/// <summary>
/// Factory class for creating IToeRun instances
/// </summary>
public class ToeRunFactory : IToeRunFactory
{
    private readonly ToeRunnerConfig _config;
    private readonly ICloudPlatform? _cloudPlatform;

    /// <summary>
    /// Constructor for ToeRunFactory
    /// </summary>
    /// <param name="config">Configuration for the ToeRunner</param>
    /// <param name="cloudPlatform">Cloud platform for uploading results</param>
    public ToeRunFactory(ToeRunnerConfig config, ICloudPlatform? cloudPlatform = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _cloudPlatform = cloudPlatform;
    }

    /// <summary>
    /// Creates a new IToeRun instance
    /// </summary>
    /// <param name="job">The ToeJob to be executed</param>
    /// <param name="id">Unique identifier for the run</param>
    /// <returns>An IToeRun implementation</returns>
    public IToeRun Create(ToeJob job, int id)
    {
        return Create(job, id, null);
    }
    
    /// <summary>
    /// Creates a new IToeRun instance with a batch ID
    /// </summary>
    /// <param name="job">The ToeJob to be executed</param>
    /// <param name="id">Unique identifier for the run</param>
    /// <param name="batchToeRunId">The ID of the batch run in Firebase</param>
    /// <returns>An IToeRun implementation</returns>
    public IToeRun Create(ToeJob job, int id, string? batchToeRunId)
    {
        return new ToeRunImplementation(
            _config, 
            job, 
            id, 
            batchToeRunId ?? string.Empty,
            _cloudPlatform);
    }
}
