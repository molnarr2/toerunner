using ToeRunner.Model.Firebase;
using SysMath = System.Math;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Method 6: MCDA (Multi-Criteria Decision Analysis) - RECOMMENDED
/// Normalize each metric to 0-100 scale first, then apply weights
/// </summary>
public class Method6MCDAAnalyzer : BaseStrategyAnalyzer
{
    // Perfect thresholds for normalization
    private const double PERFECT_WIN_RATE = 0.8;
    private const double PERFECT_SHARPE = 3.0;
    private const double PERFECT_MEDIAN_PROFIT = 0.10;
    private const double PERFECT_FEE_PROFIT = 0.08;
    private const double PERFECT_CV = 2.0;
    private const double MAX_DRAWDOWN_THRESHOLD = 1.0;

    protected override double CalculateQualityScore(
        FirebaseStrategyPerformance testPerf,
        FirebaseStrategyPerformance valPerf,
        double consistencyScore)
    {
        // Method 6: MCDA (Multi-Criteria Decision Analysis)
        // Step 1: Normalize metrics to 0-100
        // Step 2: Calculate risk and outlier penalties
        // Step 3: Combine with weights
        
        // Normalize validation metrics (primary - 50% weight)
        var valWinRateScore = SysMath.Min(valPerf.WinRate / PERFECT_WIN_RATE * 100.0, 100.0);
        var valSharpeScore = SysMath.Min(valPerf.SharpeRatio / PERFECT_SHARPE * 100.0, 100.0);
        var valProfitScore = SysMath.Min(valPerf.MedianProfit / PERFECT_MEDIAN_PROFIT * 100.0, 100.0);
        
        // Normalize test metrics (secondary - 35% weight)
        var testWinRateScore = SysMath.Min(testPerf.WinRate / PERFECT_WIN_RATE * 100.0, 100.0);
        var testSharpeScore = SysMath.Min(testPerf.SharpeRatio / PERFECT_SHARPE * 100.0, 100.0);
        var testProfitScore = SysMath.Min(testPerf.MedianProfit / PERFECT_MEDIAN_PROFIT * 100.0, 100.0);
        
        // Combine test and validation scores
        var winRateScore = valWinRateScore * 0.5 + testWinRateScore * 0.35;
        var sharpeScore = valSharpeScore * 0.5 + testSharpeScore * 0.35;
        var profitScore = valProfitScore * 0.5 + testProfitScore * 0.35;
        
        // Calculate risk penalty
        var avgCV = valPerf.CoefficientOfVariation * 0.5 + testPerf.CoefficientOfVariation * 0.35;
        var avgMaxDrawdown = valPerf.MaxDrawdown * 0.5 + testPerf.MaxDrawdown * 0.35;
        
        var riskPenalty = (avgCV / PERFECT_CV * 50.0) + (avgMaxDrawdown / MAX_DRAWDOWN_THRESHOLD * 50.0);
        riskPenalty = SysMath.Min(riskPenalty, 100.0); // Cap at 100
        
        // Calculate outlier penalty
        var avgTopTwoContrib = valPerf.TopTwoSegmentContribution * 0.5 + testPerf.TopTwoSegmentContribution * 0.35;
        var outlierPenalty = SysMath.Max(0, avgTopTwoContrib - 0.4);
        
        // Convert consistency score to bonus (0 is perfect, so invert it)
        var consistencyBonus = SysMath.Max(0, 100.0 - consistencyScore);
        
        // Calculate base score with weights
        // For meme tokens, profit is more important than consistency (10% vs 20%)
        var baseScore = 
            (profitScore * 0.45) +
            (sharpeScore * 0.20) +
            (winRateScore * 0.25) +
            (consistencyBonus * 0.10);
        
        // Apply penalties
        var qualityScore = SysMath.Max(0, baseScore - (riskPenalty * 0.3) - (outlierPenalty * 0.2));
        
        return qualityScore;
    }

    protected override List<string> GenerateValidationNotes(
        FirebaseStrategyPerformance testPerf,
        FirebaseStrategyPerformance valPerf,
        double consistencyScore)
    {
        var notes = new List<string>();
        
        if (testPerf.MedianProfit <= 0)
        {
            notes.Add("WARNING: Test median profit is not positive");
        }
        
        if (valPerf.MedianProfit <= 0)
        {
            notes.Add("WARNING: Validation median profit is not positive");
        }
        
        if (testPerf.TopTwoSegmentContribution > 0.6)
        {
            notes.Add($"WARNING: Test top two segment contribution is {testPerf.TopTwoSegmentContribution:F1}% (>60%)");
        }
        
        if (valPerf.TopTwoSegmentContribution > 0.6)
        {
            notes.Add($"WARNING: Validation top two segment contribution is {valPerf.TopTwoSegmentContribution:F1}% (>60%)");
        }
        
        if (consistencyScore > 40.0)
        {
            notes.Add($"WARNING: High inconsistency score ({consistencyScore:F1}) - possible overfitting");
        }
        else if (consistencyScore <= 20.0)
        {
            notes.Add($"GOOD: Low inconsistency score ({consistencyScore:F1})");
        }
        
        if (valPerf.WinRate >= 0.7)
        {
            notes.Add($"GOOD: High validation win rate ({valPerf.WinRate:F1}%)");
        }
        
        if (valPerf.SharpeRatio >= 2.0)
        {
            notes.Add($"GOOD: Excellent Sharpe ratio ({valPerf.SharpeRatio:F2})");
        }
        
        if (valPerf.CoefficientOfVariation > 2.0)
        {
            notes.Add($"WARNING: High coefficient of variation ({valPerf.CoefficientOfVariation:F2}) - high risk");
        }
        
        return notes;
    }
}
