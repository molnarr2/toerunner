using ToeRunner.Model.BigToe;
using Google.Cloud.Firestore;

namespace ToeRunner.Model.Firebase;


[FirestoreData]
public class FirebaseStrategyResult {
    [FirestoreProperty("id")]
    public String Id { get; set; }
    [FirestoreProperty("name")]
    public string Name { get; set; }

    [FirestoreProperty("executorContainerConfig")]
    public ExecutorContainerConfig ExecutorContainerConfig { get; set; }
    
    [FirestoreProperty("segmentStats")]
    public List<FirebaseSegmentExecutorStats> SegmentStats { get; set; } = new List<FirebaseSegmentExecutorStats>();

    [FirestoreProperty("totalTrades")]
    public int TotalTrades { get; set; }
    [FirestoreProperty("segmentCount")]
    public int SegmentCount { get; set; }
    
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
