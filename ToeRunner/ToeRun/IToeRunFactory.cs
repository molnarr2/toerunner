using ToeRunner.Model;

namespace ToeRunner.ToeRun;

/// <summary>
/// Interface for a factory that creates IToeRun instances
/// </summary>
public interface IToeRunFactory
{
    /// <summary>
    /// Creates a new IToeRun instance
    /// </summary>
    /// <param name="job">The ToeJob to be executed</param>
    /// <param name="id">Unique identifier for the run</param>
    /// <returns>An IToeRun implementation</returns>
    IToeRun Create(ToeJob job, int id);
}
