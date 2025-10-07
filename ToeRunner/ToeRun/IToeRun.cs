using ToeRunner.Conversion;

namespace ToeRunner.ToeRun;

public interface IToeRun {
    Task RunAsync();
    int GetStrategyCount();
    int GetUploadedStrategyCount();
    List<StrategyResultWithSegmentStats> GetFilteredStrategies();
}