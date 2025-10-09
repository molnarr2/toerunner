using ToeRunner.Model.Firebase;
using SysMath = System.Math;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Method 6: MCDA (Multi-Criteria Decision Analysis) - RECOMMENDED
/// Normalize each metric to 0-100 scale first, then apply weights
/// </summary>
public class Method6MCDAAnalyzer : IStrategyAnalyzer
{
    private const double TEST_SPLIT_RATIO = 0.8;
    private const double VALIDATION_SPLIT_RATIO = 0.2;
    
    // Perfect thresholds for normalization
    private const double PERFECT_WIN_RATE = 80.0;
    private const double PERFECT_SHARPE = 3.0;
    private const double PERFECT_MEDIAN_PROFIT = 0.10;
    private const double PERFECT_FEE_PROFIT = 0.08;
    private const double PERFECT_CV = 2.0;
    private const double MAX_DRAWDOWN_THRESHOLD = 1.0;

    public FirebaseStrategyPerformance? GeneratePerformance(List<FirebaseSegmentExecutorStats> segmentStats, double feePerTrade)
    {
        if (segmentStats == null || segmentStats.Count == 0)
        {
            return null;
        }

        // Get profits based on fee
        var profits = GetProfitsForFee(segmentStats, feePerTrade);
        
        // Apply hard filters
        if (!PassesHardFilters(profits))
        {
            return null;
        }

        var performance = CalculatePerformance(segmentStats, profits);
        return performance;
    }

    public FirebaseStrategyValidation GenerateValidation(List<FirebaseSegmentExecutorStats> testSegmentStats, List<FirebaseSegmentExecutorStats> validationSegmentStats, double feePerTrade)
    {
        var validation = new FirebaseStrategyValidation();
        
        // Calculate test performance
        var testProfits = GetProfitsForFee(testSegmentStats, feePerTrade);
        validation.TestPerformance = CalculatePerformance(testSegmentStats, testProfits);
        
        // Calculate validation performance
        var valProfits = GetProfitsForFee(validationSegmentStats, feePerTrade);
        validation.ValidationPerformance = CalculatePerformance(validationSegmentStats, valProfits);
        
        // Calculate consistency score
        validation.ConsistencyScore = CalculateConsistencyScore(
            validation.TestPerformance, 
            validation.ValidationPerformance);
        
        // Calculate quality score using Method 6 (MCDA) formula
        validation.QualityScore = CalculateQualityScore(
            validation.TestPerformance,
            validation.ValidationPerformance,
            validation.ConsistencyScore);
        
        // Add validation notes
        validation.ValidationNotes = GenerateValidationNotes(
            validation.TestPerformance,
            validation.ValidationPerformance,
            validation.ConsistencyScore);
        
        return validation;
    }

    private List<double> GetProfitsForFee(List<FirebaseSegmentExecutorStats> segments, double feePerTrade)
    {
        var profits = new List<double>();
        
        foreach (var segment in segments)
        {
            double profit = feePerTrade switch
            {
                0.0 => segment.TotalProfit00,
                0.001 => segment.TotalProfit001,
                0.008 => segment.TotalProfit08,
                0.01 => segment.TotalProfit10,
                0.015 => segment.TotalProfit15,
                0.02 => segment.TotalProfit20,
                0.025 => segment.TotalProfit25,
                _ => segment.TotalProfit10 // Default to 1%
            };
            profits.Add(profit);
        }
        
        return profits;
    }

    private bool PassesHardFilters(List<double> profits)
    {
        if (profits.Count == 0) return false;
        
        // Median profit must be positive
        var medianProfit = CalculateMedian(profits);
        if (medianProfit <= 0) return false;
        
        // Top two segment contribution must be <= 60%
        var topTwoContribution = CalculateTopTwoSegmentContribution(profits);
        if (topTwoContribution > 60.0) return false;
        
        return true;
    }

    private FirebaseStrategyPerformance CalculatePerformance(
        List<FirebaseSegmentExecutorStats> segments, 
        List<double> profits)
    {
        var performance = new FirebaseStrategyPerformance
        {
            SegmentCount = segments.Count
        };

        if (profits.Count == 0)
        {
            return performance;
        }

        // Win rate: percentage of segments with positive profit
        performance.WinRate = profits.Count(p => p > 0) * 100.0 / profits.Count;
        
        // Mean profit
        performance.MeanProfit = profits.Average();
        
        // Median profit
        performance.MedianProfit = CalculateMedian(profits);
        
        // Standard deviation
        performance.StdDevProfit = CalculateStdDev(profits, performance.MeanProfit);
        
        // Coefficient of variation
        performance.CoefficientOfVariation = SysMath.Abs(performance.MeanProfit) > 0.0001 
            ? performance.StdDevProfit / SysMath.Abs(performance.MeanProfit) 
            : 0;
        
        // Sharpe ratio (assuming risk-free rate = 0)
        performance.SharpeRatio = performance.StdDevProfit > 0.0001 
            ? performance.MeanProfit / performance.StdDevProfit 
            : 0;
        
        // Total trades
        performance.TotalTrades = segments.Sum(s => s.TotalTrades);
        
        // Profit per trade
        performance.ProfitPerTrade = performance.TotalTrades > 0 
            ? profits.Sum() / performance.TotalTrades 
            : 0;
        
        // Profit at realistic fees (use the profits passed in, which are already at the specified fee)
        performance.ProfitAtRealisticFees = profits.Sum();
        
        // Max drawdown (simplified: largest negative profit segment as percentage)
        var minProfit = profits.Min();
        performance.MaxDrawdown = minProfit < 0 ? SysMath.Abs(minProfit) : 0;
        
        // Top two segment contribution
        performance.TopTwoSegmentContribution = CalculateTopTwoSegmentContribution(profits);
        
        // Trimmed mean (remove top and bottom 10%)
        performance.TrimmedMeanProfit = CalculateTrimmedMean(profits, 0.1);
        
        return performance;
    }

    private double CalculateConsistencyScore(
        FirebaseStrategyPerformance testPerf, 
        FirebaseStrategyPerformance valPerf)
    {
        // Avoid division by zero
        if (testPerf.WinRate < 0.01 || SysMath.Abs(testPerf.MedianProfit) < 0.0001 || testPerf.SharpeRatio < 0.01)
        {
            return 0;
        }

        var winRateDiff = SysMath.Abs(testPerf.WinRate - valPerf.WinRate) / testPerf.WinRate;
        var profitDiff = SysMath.Abs(testPerf.MedianProfit - valPerf.MedianProfit) / SysMath.Abs(testPerf.MedianProfit);
        var sharpeDiff = SysMath.Abs(testPerf.SharpeRatio - valPerf.SharpeRatio) / SysMath.Max(testPerf.SharpeRatio, 0.1);
        
        var avgDiff = (winRateDiff + profitDiff + sharpeDiff) / 3.0;
        
        return SysMath.Max(0, 100.0 - (avgDiff * 100.0));
    }

    private double CalculateQualityScore(
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
        var outlierPenalty = SysMath.Max(0, avgTopTwoContrib - 40.0);
        
        // Calculate base score with weights
        var baseScore = 
            (profitScore * 0.30) +
            (sharpeScore * 0.25) +
            (winRateScore * 0.25) +
            (consistencyScore * 0.20);
        
        // Apply penalties
        var qualityScore = SysMath.Max(0, baseScore - (riskPenalty * 0.3) - (outlierPenalty * 0.2));
        
        return qualityScore;
    }

    private List<string> GenerateValidationNotes(
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
        
        if (testPerf.TopTwoSegmentContribution > 60.0)
        {
            notes.Add($"WARNING: Test top two segment contribution is {testPerf.TopTwoSegmentContribution:F1}% (>60%)");
        }
        
        if (valPerf.TopTwoSegmentContribution > 60.0)
        {
            notes.Add($"WARNING: Validation top two segment contribution is {valPerf.TopTwoSegmentContribution:F1}% (>60%)");
        }
        
        if (consistencyScore < 60.0)
        {
            notes.Add($"WARNING: Low consistency score ({consistencyScore:F1}) - possible overfitting");
        }
        else if (consistencyScore >= 80.0)
        {
            notes.Add($"GOOD: High consistency score ({consistencyScore:F1})");
        }
        
        if (valPerf.WinRate >= 70.0)
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

    private double CalculateMedian(List<double> values)
    {
        if (values.Count == 0) return 0;
        
        var sorted = values.OrderBy(v => v).ToList();
        int mid = sorted.Count / 2;
        
        if (sorted.Count % 2 == 0)
        {
            return (sorted[mid - 1] + sorted[mid]) / 2.0;
        }
        else
        {
            return sorted[mid];
        }
    }

    private double CalculateStdDev(List<double> values, double mean)
    {
        if (values.Count <= 1) return 0;
        
        var sumSquaredDiffs = values.Sum(v => SysMath.Pow(v - mean, 2));
        return SysMath.Sqrt(sumSquaredDiffs / (values.Count - 1));
    }

    private double CalculateTopTwoSegmentContribution(List<double> profits)
    {
        if (profits.Count == 0) return 0;
        
        var totalProfit = profits.Sum();
        if (SysMath.Abs(totalProfit) < 0.0001) return 0;
        
        var topTwo = profits.OrderByDescending(p => p).Take(2).Sum();
        return (topTwo / totalProfit) * 100.0;
    }

    private double CalculateTrimmedMean(List<double> values, double trimProportion)
    {
        if (values.Count == 0) return 0;
        
        var sorted = values.OrderBy(v => v).ToList();
        int trimCount = (int)(sorted.Count * trimProportion);
        
        if (trimCount * 2 >= sorted.Count) return CalculateMedian(values);
        
        var trimmed = sorted.Skip(trimCount).Take(sorted.Count - 2 * trimCount).ToList();
        return trimmed.Count > 0 ? trimmed.Average() : 0;
    }
}
