using ToeRunner.Model.Firebase;
using SysMath = System.Math;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Base class for strategy analyzers containing common functionality
/// </summary>
public abstract class BaseStrategyAnalyzer : IStrategyAnalyzer
{
    protected const double TEST_SPLIT_RATIO = 0.8;
    protected const double VALIDATION_SPLIT_RATIO = 0.2;

    public FirebaseStrategyPerformance? GeneratePerformance(List<FirebaseSegmentExecutorStats> segmentStats, double feePerTrade)
    {
        if (segmentStats == null || segmentStats.Count == 0)
        {
            return null;
        }

        // Filter out zero-trade segments
        var zeroTradeCount = segmentStats.Count(s => s.TotalTrades == 0);
        var nonZeroSegments = segmentStats.Where(s => s.TotalTrades > 0).ToList();
        
        if (nonZeroSegments.Count == 0)
        {
            return null;
        }

        // Get profits based on fee
        var profits = GetProfitsForFee(nonZeroSegments, feePerTrade);
        
        // Apply hard filters
        if (!PassesHardFilters(profits))
        {
            return null;
        }

        var performance = CalculatePerformance(nonZeroSegments, profits);
        performance.ZeroTradeSegmentCount = zeroTradeCount;
        return performance;
    }

    public FirebaseStrategyValidation GenerateValidation(List<FirebaseSegmentExecutorStats> testSegmentStats, List<FirebaseSegmentExecutorStats> validationSegmentStats, double feePerTrade)
    {
        var validation = new FirebaseStrategyValidation();
        
        // Filter out zero-trade segments from test set
        var testZeroTradeCount = testSegmentStats.Count(s => s.TotalTrades == 0);
        var testNonZeroSegments = testSegmentStats.Where(s => s.TotalTrades > 0).ToList();
        
        // Filter out zero-trade segments from validation set
        var valZeroTradeCount = validationSegmentStats.Count(s => s.TotalTrades == 0);
        var valNonZeroSegments = validationSegmentStats.Where(s => s.TotalTrades > 0).ToList();
        
        // Calculate test performance
        var testProfits = GetProfitsForFee(testNonZeroSegments, feePerTrade);
        validation.TestPerformance = CalculatePerformance(testNonZeroSegments, testProfits);
        validation.TestPerformance.ZeroTradeSegmentCount = testZeroTradeCount;
        
        // Calculate validation performance
        var valProfits = GetProfitsForFee(valNonZeroSegments, feePerTrade);
        validation.ValidationPerformance = CalculatePerformance(valNonZeroSegments, valProfits);
        validation.ValidationPerformance.ZeroTradeSegmentCount = valZeroTradeCount;
        
        // Calculate consistency score
        validation.ConsistencyScore = CalculateConsistencyScore(
            validation.TestPerformance, 
            validation.ValidationPerformance);
        
        // Calculate quality score using subclass-specific formula
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

    /// <summary>
    /// Calculates the quality score using the specific algorithm for this analyzer
    /// </summary>
    protected abstract double CalculateQualityScore(
        FirebaseStrategyPerformance testPerf,
        FirebaseStrategyPerformance valPerf,
        double consistencyScore);

    /// <summary>
    /// Generates validation notes specific to this analyzer
    /// </summary>
    protected abstract List<string> GenerateValidationNotes(
        FirebaseStrategyPerformance testPerf,
        FirebaseStrategyPerformance valPerf,
        double consistencyScore);

    protected List<double> GetProfitsForFee(List<FirebaseSegmentExecutorStats> segments, double feePerTrade)
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

    protected bool PassesHardFilters(List<double> profits)
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

    protected FirebaseStrategyPerformance CalculatePerformance(
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
        performance.WinRate = (profits.Count(p => p > 0) * 1.0) / profits.Count;
        
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
        
        // Max drawdown (simplified: largest negative profit segment)
        var minProfit = profits.Min();
        performance.MaxDrawdown = minProfit < 0 ? SysMath.Abs(minProfit) : 0;
        
        // Top two segment contribution
        performance.TopTwoSegmentContribution = CalculateTopTwoSegmentContribution(profits);
        
        // Trimmed mean (remove top and bottom 10%)
        performance.TrimmedMeanProfit = CalculateTrimmedMean(profits, 0.1);
        
        return performance;
    }

    protected double CalculateConsistencyScore(
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

    protected double CalculateMedian(List<double> values)
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

    protected double CalculateStdDev(List<double> values, double mean)
    {
        if (values.Count <= 1) return 0;
        
        var sumSquaredDiffs = values.Sum(v => SysMath.Pow(v - mean, 2));
        return SysMath.Sqrt(sumSquaredDiffs / (values.Count - 1));
    }

    protected double CalculateTopTwoSegmentContribution(List<double> profits)
    {
        if (profits.Count == 0) return 0;
        
        var totalProfit = profits.Sum();
        if (SysMath.Abs(totalProfit) < 0.0001) return 0;
        
        var topTwo = profits.OrderByDescending(p => p).Take(2).Sum();
        return (topTwo / totalProfit);
    }

    protected double CalculateTrimmedMean(List<double> values, double trimProportion)
    {
        if (values.Count == 0) return 0;
        
        var sorted = values.OrderBy(v => v).ToList();
        int trimCount = (int)(sorted.Count * trimProportion);
        
        if (trimCount * 2 >= sorted.Count) return CalculateMedian(values);
        
        var trimmed = sorted.Skip(trimCount).Take(sorted.Count - 2 * trimCount).ToList();
        return trimmed.Count > 0 ? trimmed.Average() : 0;
    }
}
