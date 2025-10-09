using ToeRunner.Model.Firebase;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Interface for strategy analysis implementations
/// </summary>
public interface IStrategyAnalyzer
{
    /// <summary>
    /// Generates a FirebaseStrategyPerformance if the strategy passes the test
    /// </summary>
    /// <param name="segmentStats">The segment stats to analyze</param>
    /// <param name="feePerTrade">Fee per buy/sell (e.g., 0.01 = 1%)</param>
    /// <returns>FirebaseStrategyPerformance if strategy passes test, null otherwise</returns>
    FirebaseStrategyPerformance? GeneratePerformance(List<FirebaseSegmentExecutorStats> segmentStats, double feePerTrade);
    
    /// <summary>
    /// Generates a FirebaseStrategyValidation always
    /// </summary>
    /// <param name="testSegmentStats">The test segment stats</param>
    /// <param name="validationSegmentStats">The validation segment stats</param>
    /// <param name="feePerTrade">Fee per buy/sell (e.g., 0.01 = 1%)</param>
    /// <returns>FirebaseStrategyValidation with test and validation performance</returns>
    FirebaseStrategyValidation GenerateValidation(List<FirebaseSegmentExecutorStats> testSegmentStats, List<FirebaseSegmentExecutorStats> validationSegmentStats, double feePerTrade);
}
