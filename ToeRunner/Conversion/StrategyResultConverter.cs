using System;
using System.Collections.Generic;
using System.Linq;
using ToeRunner.Math;
using ToeRunner.Model.BigToe;
using ToeRunner.Model.Firebase;

namespace ToeRunner.Conversion
{
    /// <summary>
    /// Data structure containing a FirebaseStrategyResult and its associated segment stats
    /// </summary>
    public class StrategyResultWithSegmentStats
    {
        public FirebaseStrategyResult StrategyResult { get; set; }
        public List<FirebaseSegmentExecutorStats> SegmentStats { get; set; }
        
        public StrategyResultWithSegmentStats()
        {
            StrategyResult = new FirebaseStrategyResult();
            SegmentStats = new List<FirebaseSegmentExecutorStats>();
        }
    }
    
    /// <summary>
    /// Static converter class for transforming StrategyEvaluationResult objects into StrategyResult objects
    /// </summary>
    public static class StrategyResultConverter
    {
        /// <summary>
        /// Converts a StrategyEvaluationResult to a List of StrategyResultWithSegmentStats
        /// </summary>
        /// <param name="evaluationResult">The StrategyEvaluationResult to convert</param>
        /// <param name="runName">The name of the run</param>
        /// <param name="candlestick">The candlestick value</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="batchToeRunId">The batch toe run identifier</param>
        /// <returns>A List of StrategyResultWithSegmentStats objects</returns>
        public static List<StrategyResultWithSegmentStats> ConvertToStrategyResults(StrategyEvaluationResult evaluationResult, string runName, int candlestick, string userId, string batchToeRunId)
        {
            if (evaluationResult == null || evaluationResult.ExecutorEvaluationResults == null)
            {
                return new List<StrategyResultWithSegmentStats>();
            }

            var results = new List<StrategyResultWithSegmentStats>();

            foreach (var executorEvalResult in evaluationResult.ExecutorEvaluationResults)
            {
                // Create a new StrategyResult for each ExecutorEvaluationResult
                var strategyResult = new FirebaseStrategyResult
                {
                    Id = Guid.NewGuid().ToString(),
                    RunName = runName,
                    Candlestick = candlestick,
                    SegmentCount = executorEvalResult.SegmentStats?.Count ?? 0,
                    TotalTrades = TradeCalculator.CountTotalTrades(executorEvalResult),
                    SegmentIds = GetSegmentIds(evaluationResult?.SegmentDetails)
                };
                
                // Convert segment stats separately
                var segmentStats = ConvertToFirebaseSegmentExecutorStats(executorEvalResult?.SegmentStats, strategyResult.Id, strategyResult.SegmentIds, userId, batchToeRunId);

                // Find the matching ExecutorContainerConfig
                if (evaluationResult?.TradeContainerConfig?.Executors != null)
                {
                    strategyResult.ExecutorContainerConfig = evaluationResult.TradeContainerConfig.Executors
                        .FirstOrDefault(e => e?.Name == executorEvalResult?.ExecutorName);
                }

                // Calculate profit fields using TradeCalculator
                // The fee percentages are specified as decimal values where 0.008 = 0.8%
                strategyResult.TotalProfit00 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.00m);
                strategyResult.TotalProfit08 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.008m);
                strategyResult.TotalProfit10 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.01m);
                strategyResult.TotalProfit15 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.015m);
                strategyResult.TotalProfit20 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.02m);
                strategyResult.TotalProfit25 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.025m);
                strategyResult.TotalProfit30 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.03m);
                strategyResult.TotalProfit35 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.035m);
                strategyResult.TotalProfit40 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.04m);
                strategyResult.TotalProfit50 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.05m);
                strategyResult.TotalProfit60 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult!, 0.06m);

                // Create the combined result
                var resultWithSegmentStats = new StrategyResultWithSegmentStats
                {
                    StrategyResult = strategyResult,
                    SegmentStats = segmentStats
                };
                
                results.Add(resultWithSegmentStats);
            }

            return results;
        }
        
        /// <summary>
        /// Converts a list of PlaybackSegmentDetails to a list of segment IDs
        /// </summary>
        /// <param name="segmentDetails">The list of PlaybackSegmentDetails</param>
        /// <returns>A list of segment IDs</returns>
        private static List<string> GetSegmentIds(List<PlaybackSegmentDetails>? segmentDetails)
        {
            if (segmentDetails == null || segmentDetails.Count == 0)
            {
                return new List<string>();
            }

            // Extract IDs and filter out null or empty ones
            return segmentDetails
                .Where(sd => sd != null && !string.IsNullOrEmpty(sd.Id))
                .Select(sd => sd.Id)
                .ToList();
        }
        
        /// <summary>
        /// Converts a list of SegmentExecutorStats to a list of FirebaseSegmentExecutorStats
        /// </summary>
        /// <param name="segmentStats">The list of SegmentExecutorStats to convert</param>
        /// <param name="strategyResultReplayId">The strategy result replay identifier</param>
        /// <param name="segmentIds">The list of segment IDs</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="batchToeRunId">The batch toe run identifier</param>
        /// <returns>A list of FirebaseSegmentExecutorStats</returns>
        private static List<FirebaseSegmentExecutorStats> ConvertToFirebaseSegmentExecutorStats(List<SegmentExecutorStats>? segmentStats, string strategyResultReplayId, List<string> segmentIds, string userId, string batchToeRunId)
        {
            if (segmentStats == null)
            {
                return new List<FirebaseSegmentExecutorStats>();
            }

            var result = new List<FirebaseSegmentExecutorStats>();

            for (int i = 0; i < segmentStats.Count; i++)
            {
                var segmentStat = segmentStats[i];
                if (segmentStat?.ExecutorStats?.TradeStatsList == null)
                {
                    continue;
                }
                
                // At this point we know ExecutorStats and TradeStatsList are not null
                var executorStats = segmentStat.ExecutorStats!;

                var firebaseSegmentStats = new FirebaseSegmentExecutorStats
                {
                    Id = Guid.NewGuid().ToString(),
                    StrategyResultReplayId = strategyResultReplayId,
                    SegmentId = segmentIds[i],
                    UserId = userId,
                    BatchToeRunId = batchToeRunId,
                    TotalTrades = TradeCalculator.CountSegmentTrades(segmentStat),
                    TradeStatsList = new List<FirebaseTradeStats>(),
                    
                    // Calculate profit fields using TradeCalculator at different fee percentages
                    TotalProfit00 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.00m),
                    TotalProfit08 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.008m),
                    TotalProfit10 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.01m),
                    TotalProfit15 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.015m),
                    TotalProfit20 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.02m),
                    TotalProfit25 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.025m),
                    TotalProfit30 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.03m),
                    TotalProfit35 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.035m),
                    TotalProfit40 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.04m),
                    TotalProfit50 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.05m),
                    TotalProfit60 = (double)TradeCalculator.CalculateSegmentProfit(segmentStat, 0.06m)
                };

                foreach (var tradeStat in executorStats.TradeStatsList)
                {
                    if (tradeStat == null)
                    {
                        continue;
                    }

                    var firebaseTradeStat = new FirebaseTradeStats();

                    // Convert BuyStats if available
                    if (tradeStat.BuyStats != null)
                    {
                        firebaseTradeStat.BuyStats = new FirebaseBuyStats
                        {
                            AmountBought = tradeStat.BuyStats.AmountBought,
                            TotalCost = tradeStat.BuyStats.TotalCost,
                            EndTime = tradeStat.BuyStats.EndTime
                        };
                    }

                    // Convert SellStats if available
                    if (tradeStat.SellStats != null)
                    {
                        firebaseTradeStat.SellStats = new FirebaseSellStats
                        {
                            AmountSold = tradeStat.SellStats.AmountSold,
                            TotalReceived = tradeStat.SellStats.TotalReceived,
                            EndTime = tradeStat.SellStats.EndTime
                        };
                    }

                    firebaseSegmentStats.TradeStatsList.Add(firebaseTradeStat);
                }

                result.Add(firebaseSegmentStats);
            }

            return result;
        }
    }
}
