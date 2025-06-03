namespace ToeRunner.Model.BigToe;


/// <summary>
/// Contains statistics for a buy-sell trade pair
/// </summary>
public class TradeStats {
    /// <summary>
    /// Buy statistics for this trade
    /// </summary>
    public BuyStats? BuyStats { get; set; }
    
    /// <summary>
    /// Sell statistics for this trade (may be null if sell hasn't occurred)
    /// </summary>
    public SellStats? SellStats { get; set; }
    
    /// <summary>
    /// Creates a new instance of TradeStats with buy stats
    /// </summary>
    /// <param name="buyStats">The buy statistics</param>
    public TradeStats(BuyStats buyStats) {
        BuyStats = buyStats;
    }
    
    /// <summary>
    /// Creates a new instance of TradeStats with buy and sell stats
    /// </summary>
    /// <param name="buyStats">The buy statistics</param>
    /// <param name="sellStats">The sell statistics</param>
    public TradeStats(BuyStats buyStats, SellStats sellStats) {
        BuyStats = buyStats;
        SellStats = sellStats;
    }
}

/// <summary>
/// Contains statistics for a scheduled trade executor
/// </summary>
public class ScheduledTradeExecutorStats {
    /// <summary>
    /// The trading pair used for the scheduled trade operations
    /// </summary>
    public CryptoTradingPair TradingPair { get; set; }
    
    /// <summary>
    /// The time when the executor started
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The total running time of the executor
    /// </summary>
    public TimeSpan TotalRunningTime { get; set; }
    
    /// <summary>
    /// List of statistics for all trade operations (buy-sell pairs) performed by this executor
    /// </summary>
    public List<TradeStats> TradeStatsList { get; set; } = new List<TradeStats>();
}