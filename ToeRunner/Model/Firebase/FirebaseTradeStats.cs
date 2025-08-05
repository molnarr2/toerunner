using Google.Cloud.Firestore;
using ToeRunner.Firebase;
using ToeRunner.Model.BigToe;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Contains statistics for a buy-sell trade pair
/// </summary>
[FirestoreData]
public class FirebaseTradeStats {
    /// <summary>
    /// Buy statistics for this trade
    /// </summary>
    [FirestoreProperty("buyStats")]
    public FirebaseBuyStats BuyStats { get; set; }
    
    /// <summary>
    /// Sell statistics for this trade (may be null if sell hasn't occurred)
    /// </summary>
    [FirestoreProperty("sellStats")]
    public FirebaseSellStats SellStats { get; set; }
}
