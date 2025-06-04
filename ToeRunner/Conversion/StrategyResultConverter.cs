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
        public static List<StrategyResult> ConvertToStrategyResults(StrategyEvaluationResult evaluationResult)
        {
            if (evaluationResult == null || evaluationResult.ExecutorEvaluationResults == null)
            {
                return new List<StrategyResult>();
            }

            var results = new List<StrategyResult>();

            foreach (var executorEvalResult in evaluationResult.ExecutorEvaluationResults)
            {
                // Create a new StrategyResult for each ExecutorEvaluationResult
                var strategyResult = new StrategyResult
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = executorEvalResult.ExecutorName,
                    SegmentCount = executorEvalResult.SegmentStats?.Count ?? 0,
                    TotalTrades = TradeCalculator.CountTotalTrades(executorEvalResult),
                    ExecutorEvaluationResult = executorEvalResult
                };

                // Find the matching ExecutorContainerConfig
                if (evaluationResult.TradeContainerConfig?.Executors != null)
                {
                    strategyResult.ExecutorContainerConfig = evaluationResult.TradeContainerConfig.Executors
                        .FirstOrDefault(e => e.Name == executorEvalResult.ExecutorName);
                }

                // Calculate profit fields using TradeCalculator
                // The fee percentages are specified as decimal values where 0.008 = 0.8%
                strategyResult.TotalProfit00 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.00m);
                strategyResult.TotalProfit08 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.008m);
                strategyResult.TotalProfit10 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.01m);
                strategyResult.TotalProfit15 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.015m);
                strategyResult.TotalProfit20 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.02m);
                strategyResult.TotalProfit25 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.025m);
                strategyResult.TotalProfit30 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.03m);
                strategyResult.TotalProfit35 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.035m);
                strategyResult.TotalProfit40 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.04m);
                strategyResult.TotalProfit50 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.05m);
                strategyResult.TotalProfit60 = (double)TradeCalculator.CalculateTotalProfit(executorEvalResult, 0.06m);

                results.Add(strategyResult);
            }

            return results;
        }
    }
}
