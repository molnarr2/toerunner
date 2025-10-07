using ToeRunner.Model;
using ToeRunner.Model.BigToe;

namespace ToeRunner.ToeRun;

/// <summary>
/// Interface for a factory that creates IToeRun instances
/// </summary>
public interface IToeRunFactory
{
    /// <summary>
    /// Creates a new IToeRun instance with a batch ID
    /// </summary>
    /// <param name="job">The ToeJob to be executed</param>
    /// <param name="id">Unique identifier for the run</param>
    /// <param name="batchToeRunId">The ID of the batch run in Firebase, can be null</param>
    /// <param name="segmentConfig">Optional SegmentConfig for filtering segments based on TrainOn field</param>
    /// <returns>An IToeRun implementation</returns>
    IToeRun Create(ToeJob job, int id, string? batchToeRunId, SegmentConfig? segmentConfig);
}
