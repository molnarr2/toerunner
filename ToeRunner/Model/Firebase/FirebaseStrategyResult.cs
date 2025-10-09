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
    
    // Training profit fields
    [FirestoreProperty("p00")]
    public double TotalProfit00 { get; set; }
    [FirestoreProperty("p001")]
    public double TotalProfit001 { get; set; }
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
    
    // Testing profit fields
    [FirestoreProperty("t00")]
    public double TestProfit00 { get; set; }
    [FirestoreProperty("t001")]
    public double TestProfit001 { get; set; }
    [FirestoreProperty("t08")]
    public double TestProfit08 { get; set; }
    [FirestoreProperty("t10")]
    public double TestProfit10 { get; set; }
    [FirestoreProperty("t15")]
    public double TestProfit15 { get; set; }
    [FirestoreProperty("t20")]
    public double TestProfit20 { get; set; }
    [FirestoreProperty("t25")]
    public double TestProfit25 { get; set; }
        
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
