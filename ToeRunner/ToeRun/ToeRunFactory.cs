using ToeRunner.Model;
using ToeRunner.Model.BigToe;
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
    /// <param name="cloudPlatform">Cloud platform for uploading results, can be null</param>
    public ToeRunFactory(ToeRunnerConfig config, ICloudPlatform? cloudPlatform)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _cloudPlatform = cloudPlatform;
    }
    
    /// <summary>
    /// Creates a new IToeRun instance with a batch ID
    /// </summary>
    /// <param name="job">The ToeJob to be executed</param>
    /// <param name="id">Unique identifier for the run</param>
    /// <param name="batchToeRunId">The ID of the batch run in Firebase</param>
    /// <param name="segmentConfig">Optional SegmentConfig for filtering segments based on TrainOn field</param>
    /// <returns>An IToeRun implementation</returns>
    public IToeRun Create(ToeJob job, int id, string? batchToeRunId, SegmentConfig? segmentConfig)
    {
        return new ToeRunImplementation(
            _config, 
            job, 
            id, 
            batchToeRunId,
            _cloudPlatform,
            segmentConfig);
    }
}
