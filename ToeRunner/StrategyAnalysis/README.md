# Strategy Analysis System

This directory contains the implementation of the ToeRunner Strategy Analysis system, which evaluates and ranks trading strategies based on their performance metrics.

## Overview

The system analyzes strategies using sophisticated scoring algorithms and maintains a thread-safe collection of the top 100 strategies based on quality scores.

## Components

### 1. **StrategyAnalysisType** (Enum)
Defines the available analysis methods:
- `CompositeScoring`: Method 1 - Simple weighted sum of raw performance metrics
- `MCDA`: Method 6 - Multi-Criteria Decision Analysis (RECOMMENDED)

### 2. **IStrategyAnalyzer** (Interface)
Defines the contract for strategy analysis implementations:
- `GeneratePerformance(segmentStats, feePerTrade)`: Returns performance metrics if strategy passes hard filters, null otherwise
- `GenerateValidation(testSegmentStats, validationSegmentStats, feePerTrade)`: Always returns validation results with quality scores

### 3. **Method1CompositeAnalyzer**
Implementation using direct weighted sum of raw metrics:
- **Pros**: Simple, fast, easy to understand
- **Cons**: Arbitrary scaling factors, fragile when adding metrics
- **Formula**: `QualityScore = (WinRate × 0.25) + (SharpeRatio × 20 × 0.20) + (MedianProfit × 100 × 0.20) + (ProfitAtFees × 100 × 0.15) + (ConsistencyScore × 0.10) + (OutlierPenalty × 0.10)`

### 4. **Method6MCDAAnalyzer** (RECOMMENDED)
Implementation using normalized metrics on 0-100 scale:
- **Pros**: Mathematically sound, no arbitrary scaling, industry standard
- **Cons**: Slightly more complex
- **Process**:
  1. Normalize all metrics to 0-100 scale
  2. Calculate risk and outlier penalties
  3. Combine with weights: Profit (30%), Sharpe (25%), WinRate (25%), Consistency (20%)
  4. Apply penalties for high risk and outlier dependence

### 5. **AnalyzedStrategy** (Data Structure)
Thread-safe container holding:
- Original `StrategyResultWithSegmentStats`
- `Validation001`: Validation at 0.01% fee (used for ranking)
- `Validation08`: Validation at 0.8% fee
- `Validation15`: Validation at 1.5% fee
- `QualityScore`: The ranking score from Validation001

### 6. **StrategyAnalysisService** (Main Service)
Thread-safe service for analyzing and tracking top strategies:
- Maintains top 100 strategies ordered by quality score
- Uses semaphore for thread-safe operations
- Automatically splits segments into 80% test / 20% validation
- Automatically filters out strategies that fail hard filters (using test segments)
- Replaces lowest-scoring strategies when list is full

## Usage Example

```csharp
// Create the service with MCDA analysis (recommended)
var service = new StrategyAnalysisService(StrategyAnalysisType.MCDA);

// Add strategies from multiple threads
await Task.WhenAll(
    strategyResults.Select(strategy => service.AddStrategyAsync(strategy))
);

// Retrieve top strategies
var topStrategies = await service.GetTopStrategiesAsync();

// Get statistics
var count = await service.GetCountAsync();
var minScore = await service.GetMinimumQualityScoreAsync();

// Access individual strategy data
foreach (var analyzed in topStrategies)
{
    var strategyResult = analyzed.StrategyResult;
    var qualityScore = analyzed.QualityScore;
    var validation001 = analyzed.Validation001;
    var validation08 = analyzed.Validation08;
    var validation15 = analyzed.Validation15;
    
    // Access test and validation performance
    var testPerf = validation001.TestPerformance;
    var valPerf = validation001.ValidationPerformance;
    var consistencyScore = validation001.ConsistencyScore;
    var notes = validation001.ValidationNotes;
}
```

## Hard Filters

Strategies must pass these filters to be considered:
1. **Median Profit > 0**: Both test and validation median profit must be positive
2. **Top Two Segment Contribution ≤ 60%**: No more than 60% of profit from best 2 segments
3. **Profit at Realistic Fees > 0**: Must be profitable at 0.01% fee level

## Performance Metrics Calculated

For each strategy, the system calculates:
- **Win Rate**: Percentage of profitable segments
- **Mean Profit**: Average profit across segments
- **Median Profit**: Median profit (robust to outliers)
- **Standard Deviation**: Profit variability
- **Coefficient of Variation**: Relative risk measure
- **Sharpe Ratio**: Risk-adjusted returns
- **Profit Per Trade**: Average profit per individual trade
- **Total Trades**: Sum of all trades across segments
- **Profit at Realistic Fees**: Total profit at specified fee level
- **Max Drawdown**: Largest negative profit segment
- **Top Two Segment Contribution**: Percentage of profit from best 2 segments
- **Trimmed Mean**: Mean after removing top/bottom 10%

## Consistency Score

Measures similarity between test and validation performance:
- **100**: Identical performance
- **80-100**: Very consistent (acceptable)
- **60-80**: Moderately consistent (caution)
- **< 60**: Inconsistent (likely overfit)

## Configuration

### Fee Levels
The system analyzes strategies at three fee levels:
- **0.01% (0.0001)**: Used for filtering and primary ranking
- **0.8% (0.008)**: Realistic fee scenario
- **1.5% (0.015)**: High fee scenario

### Test/Validation Split
- **80%** of segments used for testing
- **20%** of segments used for validation
- Validation performance weighted more heavily (50% vs 35%)

### Top Strategies Limit
- Maximum of **100 strategies** maintained
- Ordered by quality score (descending)
- Automatically removes lowest-scoring when adding better strategies

## Thread Safety

All public methods are thread-safe:
- Uses `SemaphoreSlim` for async coordination
- Internal lock for list modifications
- Safe to call from multiple threads simultaneously

## Validation Notes

The system generates helpful notes for each strategy:
- Warnings for failed filters or concerning metrics
- Positive notes for excellent performance
- Consistency warnings for potential overfitting
- Specific metric values for transparency
