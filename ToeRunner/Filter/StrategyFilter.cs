using System;
using System.Collections.Generic;
using System.Linq;
using ToeRunner.Model;
using ToeRunner.Model.Firebase;

namespace ToeRunner.Filter
{
    /// <summary>
    /// Static class for filtering strategy results based on profit metrics
    /// </summary>
    public static class StrategyFilter
    {
        /// <summary>
        /// Filters out failed strategies based on profit metrics and returns a percentage of the best performing strategies
        /// </summary>
        /// <param name="strategies">List of strategy results to filter</param>
        /// <param name="uploadStrategyPercentage">Percentage of strategies to keep (1.0 = 100%, 0.2 = 20%)</param>
        /// <param name="filterPercentageType">The profit percentage type to use for filtering</param>
        /// <returns>Filtered list of strategy results</returns>
        public static List<StrategyResult> FilterFailedStrategies(
            List<StrategyResult> strategies, 
            decimal uploadStrategyPercentage, 
            FilterPercentageType filterPercentageType)
        {
            if (strategies == null || !strategies.Any())
            {
                return new List<StrategyResult>();
            }

            // Step 1: Filter out failed strategies (zero or negative profit)
            var successfulStrategies = strategies.Where(s => GetProfitByType(s, filterPercentageType) > 0).ToList();
            
            if (!successfulStrategies.Any())
            {
                return new List<StrategyResult>();
            }

            // Step 2: Sort by the profit field corresponding to the filter type
            successfulStrategies = successfulStrategies
                .OrderByDescending(s => GetProfitByType(s, filterPercentageType))
                .ToList();

            // Step 3: Keep only the top percentage based on uploadStrategyPercentage
            if (uploadStrategyPercentage >= 1.0m)
            {
                // Keep all strategies if percentage is 100% or higher
                return successfulStrategies;
            }
            
            // Calculate how many strategies to keep
            int strategiesToKeep = (int)System.Math.Ceiling(successfulStrategies.Count * (double)uploadStrategyPercentage);
            strategiesToKeep = System.Math.Max(1, strategiesToKeep); // Ensure we keep at least one strategy
            
            // Return the top performing strategies
            return successfulStrategies.Take(strategiesToKeep).ToList();
        }

        /// <summary>
        /// Gets the profit value from a strategy result based on the filter percentage type
        /// </summary>
        /// <param name="strategy">The strategy result</param>
        /// <param name="filterPercentageType">The filter percentage type</param>
        /// <returns>The profit value</returns>
        private static double GetProfitByType(StrategyResult strategy, FilterPercentageType filterPercentageType)
        {
            return filterPercentageType switch
            {
                FilterPercentageType.p00 => strategy.TotalProfit00,
                FilterPercentageType.p08 => strategy.TotalProfit08,
                FilterPercentageType.p10 => strategy.TotalProfit10,
                FilterPercentageType.p15 => strategy.TotalProfit15,
                FilterPercentageType.p20 => strategy.TotalProfit20,
                FilterPercentageType.p25 => strategy.TotalProfit25,
                FilterPercentageType.p30 => strategy.TotalProfit30,
                FilterPercentageType.p35 => strategy.TotalProfit35,
                FilterPercentageType.p40 => strategy.TotalProfit40,
                FilterPercentageType.p50 => strategy.TotalProfit50,
                FilterPercentageType.p60 => strategy.TotalProfit60,
                _ => throw new ArgumentException($"Unsupported filter percentage type: {filterPercentageType}")
            };
        }
    }
}
