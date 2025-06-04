using System;
using System.Linq;
using ToeRunner.Model.BigToe;

namespace ToeRunner.Math
{
    /// <summary>
    /// Static class for calculating trade profits and related metrics
    /// </summary>
    public static class TradeCalculator
    {
        /// <summary>
        /// Calculates the profit for a single trade, applying the specified fee percentage
        /// </summary>
        /// <param name="trade">The trade statistics containing buy and sell information</param>
        /// <param name="feePercentage">Fee percentage as a decimal (e.g., 0.01 = 1%)</param>
        /// <returns>The profit amount in quote currency, or 0 if the trade is incomplete or unsuccessful</returns>
        public static decimal CalculateTradeProfit(TradeStats trade, decimal feePercentage)
        {
            // If either buy or sell is missing or unsuccessful, there's no profit
            if (trade.BuyStats == null || trade.SellStats == null ||
                !trade.BuyStats.IsSuccessful || !trade.SellStats.IsSuccessful)
            {
                return 0;
            }

            // Calculate fees based on the provided fee percentage
            decimal buyFee = trade.BuyStats.TotalCost * feePercentage;
            decimal sellFee = trade.SellStats.TotalReceived * feePercentage;

            // Calculate profit: (Total received from sell - sell fees) - (Total cost of buy + buy fees)
            decimal sellAmount = trade.SellStats.TotalReceived - sellFee;
            decimal buyAmount = trade.BuyStats.TotalCost + buyFee;

            return sellAmount - buyAmount;
        }

        /// <summary>
        /// Calculates the profit percentage relative to the initial investment
        /// </summary>
        /// <param name="trade">The trade statistics containing buy and sell information</param>
        /// <param name="feePercentage">Fee percentage as a decimal (e.g., 0.01 = 1%)</param>
        /// <returns>The profit percentage as a decimal (e.g., 0.1 = 10%), or 0 if the trade is incomplete or unsuccessful</returns>
        public static decimal CalculateTradeProfitPercentage(TradeStats trade, decimal feePercentage)
        {
            // If either buy or sell is missing or unsuccessful, there's no profit
            if (trade.BuyStats == null || trade.SellStats == null ||
                !trade.BuyStats.IsSuccessful || !trade.SellStats.IsSuccessful)
            {
                return 0;
            }

            decimal profit = CalculateTradeProfit(trade, feePercentage);
            decimal investment = trade.BuyStats.TotalCost;

            // Avoid division by zero
            if (investment == 0)
            {
                return 0;
            }

            return profit / investment;
        }

        /// <summary>
        /// Calculates the total profit for a single segment
        /// </summary>
        /// <param name="segment">The segment executor stats containing trade information</param>
        /// <param name="feePercentage">Fee percentage as a decimal (e.g., 0.01 = 1%)</param>
        /// <returns>The total profit for the segment, or 0 if there are no trades</returns>
        public static decimal CalculateSegmentProfit(SegmentExecutorStats segment, decimal feePercentage)
        {
            if (segment.ExecutorStats == null || segment.ExecutorStats.TradeStatsList == null ||
                segment.ExecutorStats.TradeStatsList.Count == 0)
            {
                return 0;
            }

            // Calculate profit for each trade in this segment and sum them up
            return segment.ExecutorStats.TradeStatsList.Sum(trade => CalculateTradeProfit(trade, feePercentage));
        }

        /// <summary>
        /// Calculates the total profit from all trades in an executor evaluation result
        /// </summary>
        /// <param name="executorResult">The executor evaluation result containing segment stats</param>
        /// <param name="feePercentage">Fee percentage as a decimal (e.g., 0.01 = 1%)</param>
        /// <returns>The total profit across all segments and trades</returns>
        public static decimal CalculateTotalProfit(ExecutorEvaluationResult executorResult, decimal feePercentage)
        {
            if (executorResult.SegmentStats == null || executorResult.SegmentStats.Count == 0)
            {
                return 0;
            }

            // Sum up profits from all trades across all segments
            decimal totalProfit = 0;
            
            foreach (var segment in executorResult.SegmentStats)
            {
                // Skip if executor stats or trade stats list is missing
                if (segment.ExecutorStats == null || segment.ExecutorStats.TradeStatsList == null)
                {
                    continue;
                }

                // Calculate profit for each trade in this segment and add to total
                foreach (var trade in segment.ExecutorStats.TradeStatsList)
                {
                    totalProfit += CalculateTradeProfit(trade, feePercentage);
                }
            }

            return totalProfit;
        }

        /// <summary>
        /// Counts the total number of trades in an executor evaluation result
        /// </summary>
        /// <param name="executorResult">The executor evaluation result containing segment stats</param>
        /// <returns>The total number of trades across all segments</returns>
        public static int CountTotalTrades(ExecutorEvaluationResult executorResult)
        {
            if (executorResult.SegmentStats == null || executorResult.SegmentStats.Count == 0)
            {
                return 0;
            }

            // Count all trades across all segments
            int totalCount = 0;
            
            foreach (var segment in executorResult.SegmentStats)
            {
                // Skip if executor stats or trade stats list is missing
                if (segment.ExecutorStats == null || segment.ExecutorStats.TradeStatsList == null)
                {
                    continue;
                }

                // Add the number of trades in this segment to the total count
                totalCount += segment.ExecutorStats.TradeStatsList.Count;
            }

            return totalCount;
        }

        /// <summary>
        /// Calculates the average profit per trade
        /// </summary>
        /// <param name="executorResult">The executor evaluation result containing segment stats</param>
        /// <param name="feePercentage">Fee percentage as a decimal (e.g., 0.01 = 1%)</param>
        /// <returns>The average profit per trade, or 0 if there are no trades</returns>
        public static decimal CalculateAverageProfitPerTrade(ExecutorEvaluationResult executorResult, decimal feePercentage)
        {
            int totalTrades = CountTotalTrades(executorResult);
            
            if (totalTrades == 0)
            {
                return 0;
            }

            decimal totalProfit = CalculateTotalProfit(executorResult, feePercentage);
            return totalProfit / totalTrades;
        }

        /// <summary>
        /// Calculates the average profit per segment
        /// </summary>
        /// <param name="executorResult">The executor evaluation result containing segment stats</param>
        /// <param name="feePercentage">Fee percentage as a decimal (e.g., 0.01 = 1%)</param>
        /// <returns>The average profit per segment, or 0 if there are no segments</returns>
        public static decimal CalculateAverageSegmentProfit(ExecutorEvaluationResult executorResult, decimal feePercentage)
        {
            if (executorResult.SegmentStats == null || executorResult.SegmentStats.Count == 0)
            {
                return 0;
            }

            // Calculate profit for each segment
            var segmentProfits = executorResult.SegmentStats
                .Select(segment => CalculateSegmentProfit(segment, feePercentage))
                .ToList();

            // Filter out segments with no trades to avoid skewing the results
            var validSegments = segmentProfits.Where(profit => profit != 0).ToList();

            // If we don't have any valid segments with profit, return 0
            if (validSegments.Count == 0)
            {
                return 0;
            }

            // Calculate the average profit per segment
            return validSegments.Sum() / validSegments.Count;
        }
    }
}
