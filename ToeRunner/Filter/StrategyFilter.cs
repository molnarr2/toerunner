using System;
using System.Collections.Generic;
using System.Linq;
using ToeRunner.Conversion;
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
        /// <param name="strategiesWithStats">List of strategy results with segment stats to filter</param>
        /// <param name="uploadStrategyPercentage">Percentage of strategies to keep (1.0 = 100%, 0.2 = 20%)</param>
        /// <param name="filterPercentageType">The profit percentage type to use for filtering</param>
        /// <returns>Filtered list of strategy results with segment stats</returns>
        public static List<StrategyResultWithSegmentStats> FilterFailedStrategies(
            List<StrategyResultWithSegmentStats> strategiesWithStats, 
            decimal uploadStrategyPercentage, 
            FilterPercentageType filterPercentageType)
        {
            if (strategiesWithStats == null || !strategiesWithStats.Any())
            {
                return new List<StrategyResultWithSegmentStats>();
            }

            // Step 1: Filter out failed strategies (zero or negative profit)
            var successfulStrategies = strategiesWithStats.Where(s => GetProfitByType(s.StrategyResult, filterPercentageType) > 0).ToList();
            
            if (!successfulStrategies.Any())
            {
                return new List<StrategyResultWithSegmentStats>();
            }

            // Step 2: Sort by the profit field corresponding to the filter type
            successfulStrategies = successfulStrategies
                .OrderByDescending(s => GetProfitByType(s.StrategyResult, filterPercentageType))
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
        /// <param name="firebaseStrategy">The strategy result</param>
        /// <param name="filterPercentageType">The filter percentage type</param>
        /// <returns>The profit value</returns>
        private static double GetProfitByType(FirebaseStrategyResult firebaseStrategy, FilterPercentageType filterPercentageType)
        {
            return filterPercentageType switch
            {
                FilterPercentageType.p00 => firebaseStrategy.TotalProfit00,
                FilterPercentageType.p08 => firebaseStrategy.TotalProfit08,
                FilterPercentageType.p10 => firebaseStrategy.TotalProfit10,
                FilterPercentageType.p15 => firebaseStrategy.TotalProfit15,
                FilterPercentageType.p20 => firebaseStrategy.TotalProfit20,
                FilterPercentageType.p25 => firebaseStrategy.TotalProfit25,
                FilterPercentageType.p30 => firebaseStrategy.TotalProfit30,
                FilterPercentageType.p35 => firebaseStrategy.TotalProfit35,
                FilterPercentageType.p40 => firebaseStrategy.TotalProfit40,
                FilterPercentageType.p50 => firebaseStrategy.TotalProfit50,
                FilterPercentageType.p60 => firebaseStrategy.TotalProfit60,
                _ => throw new ArgumentException($"Unsupported filter percentage type: {filterPercentageType}")
            };
        }
    }
}
