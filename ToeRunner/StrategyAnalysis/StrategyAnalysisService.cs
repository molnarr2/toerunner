using ToeRunner.Conversion;

namespace ToeRunner.StrategyAnalysis;

/// <summary>
/// Thread-safe service for analyzing and tracking top strategies
/// </summary>
public class StrategyAnalysisService
{
    private const int MAX_STRATEGIES = 100;
    private const double FILTER_FEE = 0.0001; // 0.01% fee for filtering
    private const double TEST_SPLIT_RATIO = 0.8;
    
    private readonly IStrategyAnalyzer _analyzer;
    private readonly SemaphoreSlim _semaphore;
    private readonly List<AnalyzedStrategy> _topStrategies;
    private readonly object _lock = new object();

    /// <summary>
    /// Creates a new StrategyAnalysisService with the specified analysis type
    /// </summary>
    /// <param name="analysisType">The type of strategy analysis to use</param>
    public StrategyAnalysisService(StrategyAnalysisType analysisType)
    {
        _analyzer = CreateAnalyzer(analysisType);
        _semaphore = new SemaphoreSlim(1, 1);
        _topStrategies = new List<AnalyzedStrategy>();
    }

    /// <summary>
    /// Adds a strategy for analysis. Thread-safe.
    /// </summary>
    /// <param name="strategyResult">The strategy result with segment stats</param>
    /// <returns>True if the strategy was added to the top list, false otherwise</returns>
    public async Task<bool> AddStrategyAsync(StrategyResultWithSegmentStats strategyResult)
    {
        await _semaphore.WaitAsync();
        try
        {
            return AddStrategyInternal(strategyResult);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Retrieves the current top strategies. Thread-safe.
    /// </summary>
    /// <returns>A copy of the top strategies list, ordered by quality score descending</returns>
    public async Task<List<AnalyzedStrategy>> GetTopStrategiesAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_lock)
            {
                return new List<AnalyzedStrategy>(_topStrategies);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets the current count of top strategies. Thread-safe.
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_lock)
            {
                return _topStrategies.Count;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets the minimum quality score in the current top list. Thread-safe.
    /// </summary>
    public async Task<double> GetMinimumQualityScoreAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_lock)
            {
                return _topStrategies.Count > 0 ? _topStrategies.Min(s => s.QualityScore) : 0;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private bool AddStrategyInternal(StrategyResultWithSegmentStats strategyResult)
    {
        // Split segments into test (80%) and validation (20%)
        var (testSegments, validationSegments) = SplitSegments(strategyResult.SegmentStats);
        
        // Step 1: Run the test with 0.01% fee to determine if it's a good strategy
        var performance = _analyzer.GeneratePerformance(testSegments, FILTER_FEE);
        
        // If performance is null, the strategy failed the hard filters
        if (performance == null)
        {
            return false;
        }

        // Step 2: Generate validation results at multiple fee levels
        var validation001 = _analyzer.GenerateValidation(testSegments, validationSegments, 0.0001); // 0.01%
        var validation08 = _analyzer.GenerateValidation(testSegments, validationSegments, 0.008);   // 0.8%
        var validation15 = _analyzer.GenerateValidation(testSegments, validationSegments, 0.015);   // 1.5%
        
        // Debug output for validation001
        Console.WriteLine($"[Validation001] QualityScore: {validation001.QualityScore:F4}, ConsistencyScore: {validation001.ConsistencyScore:F4} | Test: ProfitAtRealisticFees={validation001.TestPerformance.ProfitAtRealisticFees:F4}, WinRate={validation001.TestPerformance.WinRate:F4}, MeanProfit={validation001.TestPerformance.MeanProfit:F4}, TotalTrades={validation001.TestPerformance.TotalTrades} | Validation: ProfitAtRealisticFees={validation001.ValidationPerformance.ProfitAtRealisticFees:F4}, WinRate={validation001.ValidationPerformance.WinRate:F4}, MeanProfit={validation001.ValidationPerformance.MeanProfit:F4}, TotalTrades={validation001.ValidationPerformance.TotalTrades}");

        // Step 3: Create the analyzed strategy
        var analyzedStrategy = new AnalyzedStrategy(
            strategyResult,
            validation001,
            validation08,
            validation15);

        // Step 4: Add to the list if it qualifies
        lock (_lock)
        {
            // If list is not full, add it
            if (_topStrategies.Count < MAX_STRATEGIES)
            {
                _topStrategies.Add(analyzedStrategy);
                SortStrategies();
                return true;
            }

            // If list is full, check if new strategy is better than the worst one
            var minScore = _topStrategies.Min(s => s.QualityScore);
            if (analyzedStrategy.QualityScore > minScore)
            {
                // Remove the strategy with the lowest score
                var worstStrategy = _topStrategies.OrderBy(s => s.QualityScore).First();
                _topStrategies.Remove(worstStrategy);
                
                // Add the new strategy
                _topStrategies.Add(analyzedStrategy);
                SortStrategies();
                return true;
            }

            // New strategy is not good enough
            return false;
        }
    }

    private void SortStrategies()
    {
        // Sort by quality score descending (highest first)
        _topStrategies.Sort((a, b) => b.QualityScore.CompareTo(a.QualityScore));
    }

    private (List<ToeRunner.Model.Firebase.FirebaseSegmentExecutorStats>, List<ToeRunner.Model.Firebase.FirebaseSegmentExecutorStats>) SplitSegments(
        List<ToeRunner.Model.Firebase.FirebaseSegmentExecutorStats> segments)
    {
        var testCount = (int)System.Math.Ceiling(segments.Count * TEST_SPLIT_RATIO);
        var testSegments = segments.Take(testCount).ToList();
        var validationSegments = segments.Skip(testCount).ToList();
        
        return (testSegments, validationSegments);
    }

    private IStrategyAnalyzer CreateAnalyzer(StrategyAnalysisType analysisType)
    {
        return analysisType switch
        {
            StrategyAnalysisType.CompositeScoring => new Method1CompositeAnalyzer(),
            StrategyAnalysisType.MCDA => new Method6MCDAAnalyzer(),
            _ => throw new ArgumentException($"Unknown analysis type: {analysisType}")
        };
    }
}
