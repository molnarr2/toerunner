using ToeRunner.Conversion;
using ToeRunner.Model.Firebase;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Thread-safe data structure containing a strategy result with its validation analysis
/// </summary>
public class AnalyzedStrategy
{
    /// <summary>
    /// The original strategy result with segment stats
    /// </summary>
    public StrategyResultWithSegmentStats StrategyResult { get; set; }
    
    /// <summary>
    /// Validation results at 0.01% fee (0.0001)
    /// </summary>
    public FirebaseStrategyValidation Validation001 { get; set; }
    
    /// <summary>
    /// Validation results at 0.8% fee (0.008)
    /// </summary>
    public FirebaseStrategyValidation Validation08 { get; set; }
    
    /// <summary>
    /// Validation results at 1.5% fee (0.015)
    /// </summary>
    public FirebaseStrategyValidation Validation15 { get; set; }
    
    /// <summary>
    /// The quality score used for ranking (from Validation001)
    /// </summary>
    public double QualityScore => Validation001?.QualityScore ?? 0;

    public AnalyzedStrategy(
        StrategyResultWithSegmentStats strategyResult,
        FirebaseStrategyValidation validation001,
        FirebaseStrategyValidation validation08,
        FirebaseStrategyValidation validation15)
    {
        StrategyResult = strategyResult;
        Validation001 = validation001;
        Validation08 = validation08;
        Validation15 = validation15;
    }
}
