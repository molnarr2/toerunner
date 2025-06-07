using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;


/// <summary>
/// Contains statistics for a buy-sell trade pair
/// </summary>
[FirestoreData]
public class TradeStats {
    /// <summary>
    /// Buy statistics for this trade
    /// </summary>
    [FirestoreProperty("bs")]
    public BuyStats? BuyStats { get; set; }
    
    /// <summary>
    /// Sell statistics for this trade (may be null if sell hasn't occurred)
    /// </summary>
    [FirestoreProperty("ss")]
    public SellStats? SellStats { get; set; }
}

/// <summary>
/// Contains statistics for a scheduled trade executor
/// </summary>
[FirestoreData]
public class ScheduledTradeExecutorStats {
    /// <summary>
    /// The trading pair used for the scheduled trade operations
    /// </summary>
    [FirestoreProperty("tp")]
    public CryptoTradingPair TradingPair { get; set; }
    
    /// <summary>
    /// The time when the executor started
    /// </summary>
    [FirestoreProperty("st", ConverterType = typeof(DateTimeConverter))]
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The total running time of the executor
    /// </summary>
    [FirestoreProperty("tr", ConverterType = typeof(TimeSpanToSecondsConverter))]
    public TimeSpan TotalRunningTime { get; set; }
    
    /// <summary>
    /// List of statistics for all trade operations (buy-sell pairs) performed by this executor
    /// </summary>
    [FirestoreProperty("ts")]
    public List<TradeStats> TradeStatsList { get; set; }
}