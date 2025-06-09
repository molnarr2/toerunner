using System;
using System.Collections.Generic;
using System.Linq;
using ToeRunner.Math;
using ToeRunner.Model.BigToe;
using ToeRunner.Model.Firebase;

namespace ToeRunner.Conversion
{
    /// <summary>
    /// Static converter class for transforming StrategyEvaluationResult objects into StrategyResult objects
    /// </summary>
    public static class StrategyResultConverter
    {
        /// <summary>
        /// Converts a StrategyEvaluationResult to a List of StrategyResult
        /// </summary>
        /// <param name="evaluationResult">The StrategyEvaluationResult to convert</param>
        /// <returns>A List of StrategyResult objects</returns>
        public static List<FirebaseStrategyResult> ConvertToStrategyResults(StrategyEvaluationResult evaluationResult)
        {
            if (evaluationResult == null || evaluationResult.ExecutorEvaluationResults == null)
            {
                return new List<FirebaseStrategyResult>();
            }

            var results = new List<FirebaseStrategyResult>();

            foreach (var executorEvalResult in evaluationResult.ExecutorEvaluationResults)
            {
                // Create a new StrategyResult for each ExecutorEvaluationResult
                var strategyResult = new FirebaseStrategyResult
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = executorEvalResult.ExecutorName,
                    SegmentCount = executorEvalResult.SegmentStats?.Count ?? 0,
                    TotalTrades = TradeCalculator.CountTotalTrades(executorEvalResult),
                    SegmentStats = ConvertToFirebaseSegmentExecutorStats(executorEvalResult?.SegmentStats),
                    SegmentIds = GetSegmentIds(evaluationResult?.SegmentDetails)
                };

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

                results.Add(strategyResult);
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
        /// <returns>A list of FirebaseSegmentExecutorStats</returns>
        private static List<FirebaseSegmentExecutorStats> ConvertToFirebaseSegmentExecutorStats(List<SegmentExecutorStats>? segmentStats)
        {
            if (segmentStats == null)
            {
                return new List<FirebaseSegmentExecutorStats>();
            }

            var result = new List<FirebaseSegmentExecutorStats>();

            foreach (var segmentStat in segmentStats)
            {
                if (segmentStat?.ExecutorStats?.TradeStatsList == null)
                {
                    continue;
                }
                
                // At this point we know ExecutorStats and TradeStatsList are not null
                var executorStats = segmentStat.ExecutorStats!;

                var firebaseSegmentStats = new FirebaseSegmentExecutorStats
                {
                    TradeStatsList = new List<FirebaseTradeStats>()
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
