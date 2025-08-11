using Google.Cloud.Firestore;
using ToeRunner.Model.BigToe;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Contains statistics for a scheduled trade executor
/// </summary>
[FirestoreData]
public class FirebaseSegmentExecutorStats {
    /// <summary>
    /// Unique identifier for this segment executor stats
    /// </summary>
    [FirestoreProperty("id")]
    public String Id { get; set; } = string.Empty;
    
    /// <summary>
    /// User identifier
    /// </summary>
    [FirestoreProperty("userId")]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Trade result replay identifier
    /// </summary>
    [FirestoreProperty("strategyResultReplayId")]
    public string StrategyResultReplayId { get; set; } = string.Empty;
    
    /// <summary>
    /// Segment identifier
    /// </summary>
    [FirestoreProperty("segmentId")]
    public string SegmentId { get; set; } = string.Empty;
    
    /// <summary>
    /// Total number of trades in this segment
    /// </summary>
    [FirestoreProperty("totalTrades")]
    public int TotalTrades { get; set; }
    
    /// <summary>
    /// List of statistics for all trade operations (buy-sell pairs) performed by this executor
    /// </summary>
    [FirestoreProperty("tradeStatsList")]
    public List<FirebaseTradeStats> TradeStatsList { get; set; } = new List<FirebaseTradeStats>();
    
    // Profit calculations at different fee percentages
    [FirestoreProperty("p00")]
    public double TotalProfit00 { get; set; }
    [FirestoreProperty("p08")]
    public double TotalProfit08 { get; set; }
    [FirestoreProperty("p10")]
    public double TotalProfit10 { get; set; }
    [FirestoreProperty("p15")]
    public double TotalProfit15 { get; set; }
    [FirestoreProperty("p20")]
    public double TotalProfit20 { get; set; }
    [FirestoreProperty("p25")]
    public double TotalProfit25 { get; set; }
    [FirestoreProperty("p30")]
    public double TotalProfit30 { get; set; }
    [FirestoreProperty("p35")]
    public double TotalProfit35 { get; set; }
    [FirestoreProperty("p40")]
    public double TotalProfit40 { get; set; }
    [FirestoreProperty("p50")]
    public double TotalProfit50 { get; set; }
    [FirestoreProperty("p60")]
    public double TotalProfit60 { get; set; }
}
