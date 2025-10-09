using ToeRunner.Model.BigToe;
using Google.Cloud.Firestore;

namespace ToeRunner.Model.Firebase;


[FirestoreData]
public class FirebaseStrategyResult {
    [FirestoreProperty("id")]
    public String Id { get; set; }
    [FirestoreProperty("runName")]
    public string RunName { get; set; }
    [FirestoreProperty("candlestick")]
    public int Candlestick { get; set; }

    [FirestoreProperty("executorContainerConfig")]
    public ExecutorContainerConfig ExecutorContainerConfig { get; set; }
    
    [FirestoreProperty("totalTrades")]
    public int TotalTrades { get; set; }
    [FirestoreProperty("segmentIds")]
    public List<string> SegmentIds { get; set; } = new List<string>();

    [FirestoreProperty("segmentCount")]
    public int SegmentCount { get; set; }
    
    // Validation with 0.01% fee per buy/sell
    [FirestoreProperty("validationWithFee001")]
    public FirebaseStrategyValidation ValidationWithFee001 { get; set; }
    
    // Validation with 0.8% fee per buy/sell
    [FirestoreProperty("validationWithFee08")]
    public FirebaseStrategyValidation ValidationWithFee08 { get; set; }
    
    // Validation with 0.15% fee per buy/sell
    [FirestoreProperty("validationWithFee15")]
    public FirebaseStrategyValidation ValidationWithFee15 { get; set; }
}
