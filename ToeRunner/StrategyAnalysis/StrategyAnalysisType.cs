namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Types of strategy analysis methods available
/// </summary>
public enum StrategyAnalysisType
{
    /// <summary>
    /// Method 1: Composite Scoring (Simple Weighted Sum)
    /// Direct weighted sum of raw performance metrics
    /// </summary>
    CompositeScoring,
    
    /// <summary>
    /// Method 6: MCDA (Multi-Criteria Decision Analysis) - RECOMMENDED
    /// Normalize each metric to 0-100 scale first, then apply weights
    /// </summary>
    MCDA
}
