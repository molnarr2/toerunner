using Google.Cloud.Firestore;
using ToeRunner.Model.BigToe;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Contains statistics for a scheduled trade executor
/// </summary>
[FirestoreData]
public class FirebaseSegmentExecutorStats {
    /// <summary>
    /// List of statistics for all trade operations (buy-sell pairs) performed by this executor
    /// </summary>
    [FirestoreProperty("ts")]
    public List<FirebaseTradeStats> TradeStatsList { get; set; }
}
