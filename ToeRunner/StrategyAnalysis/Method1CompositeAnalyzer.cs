using ToeRunner.Model.Firebase;
using SysMath = System.Math;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Method 1: Composite Scoring (Simple Weighted Sum)
/// Direct weighted sum of raw performance metrics
/// </summary>
public class Method1CompositeAnalyzer : BaseStrategyAnalyzer
{
    protected override double CalculateQualityScore(
        FirebaseStrategyPerformance testPerf,
        FirebaseStrategyPerformance valPerf,
        double consistencyScore)
    {
        // Method 1: Composite Scoring (Simple Weighted Sum)
        // QualityScore = 
        //     (WinRate × 0.25) +
        //     (SharpeRatio × 20 × 0.20) +
        //     (MedianProfit × 100 × 0.20) +
        //     (ProfitAtRealisticFees × 100 × 0.15) +
        //     (ConsistencyBonus × 0.10) +  // Inverted: lower consistencyScore is better
        //     (OutlierPenalty × 0.10)
        
        // Use validation performance as primary (50% weight) and test as secondary (35%)
        var winRate = valPerf.WinRate * 0.5 + testPerf.WinRate * 0.35;
        var sharpeRatio = valPerf.SharpeRatio * 0.5 + testPerf.SharpeRatio * 0.35;
        var medianProfit = valPerf.MedianProfit * 0.5 + testPerf.MedianProfit * 0.35;
        var profitAtFees = valPerf.ProfitAtRealisticFees * 0.5 + testPerf.ProfitAtRealisticFees * 0.35;
        var topTwoContrib = valPerf.TopTwoSegmentContribution * 0.5 + testPerf.TopTwoSegmentContribution * 0.35;
        
        var outlierPenalty = 1.0 - topTwoContrib;
        
        // Invert consistency score: 0 is perfect, so we convert to bonus (100 - consistencyScore) / 100
        var consistencyBonus = SysMath.Max(0, 100.0 - consistencyScore) / 100.0;
        
        var qualityScore = 
            (winRate * 0.25) +
            (sharpeRatio * 20.0 * 0.20) +
            (medianProfit * 100.0 * 0.20) +
            (profitAtFees * 100.0 * 0.15) +
            (consistencyBonus * 0.10) +
            (outlierPenalty * 0.10);
        
        return SysMath.Max(0, qualityScore);
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
        
        return notes;
    }
}
