using ToeRunner.Model;

namespace ToeRunner.ToeRun;

/// <summary>
/// Factory class for creating IToeRun instances
/// </summary>
public class ToeRunFactory : IToeRunFactory
{
    private readonly ToeRunnerConfig _config;

    /// <summary>
    /// Constructor for ToeRunFactory
    /// </summary>
    /// <param name="config">Configuration for the ToeRunner</param>
    public ToeRunFactory(ToeRunnerConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Creates a new IToeRun instance
    /// </summary>
    /// <param name="job">The ToeJob to be executed</param>
    /// <param name="id">Unique identifier for the run</param>
    /// <returns>An IToeRun implementation</returns>
    public IToeRun Create(ToeJob job, int id)
    {
        return new ToeRunImplementation(_config, job, id);
    }
}
