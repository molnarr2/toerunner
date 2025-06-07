using ToeRunner.Model.BigToe;
using Google.Cloud.Firestore;

namespace ToeRunner.Model.Firebase;


[FirestoreData]
public class StrategyResult {
    [FirestoreProperty("id")]
    public String Id { get; set; }
    [FirestoreProperty("n")]
    public string Name { get; set; }

    [FirestoreProperty("ec")]
    public ExecutorContainerConfig ExecutorContainerConfig { get; set; }
    [FirestoreProperty("er")]
    public ExecutorEvaluationResult ExecutorEvaluationResult { get; set; }
    
    [FirestoreProperty("tt")]
    public int TotalTrades { get; set; }
    [FirestoreProperty("sc")]
    public int SegmentCount { get; set; }
    
    [FirestoreProperty("p0")]
    public double TotalProfit00 { get; set; }
    [FirestoreProperty("p8")]
    public double TotalProfit08 { get; set; }
    [FirestoreProperty("p1")]
    public double TotalProfit10 { get; set; }
    [FirestoreProperty("p15")]
    public double TotalProfit15 { get; set; }
    [FirestoreProperty("p2")]
    public double TotalProfit20 { get; set; }
    [FirestoreProperty("p25")]
    public double TotalProfit25 { get; set; }
    [FirestoreProperty("p3")]
    public double TotalProfit30 { get; set; }
    [FirestoreProperty("p35")]
    public double TotalProfit35 { get; set; }
    [FirestoreProperty("p4")]
    public double TotalProfit40 { get; set; }
    [FirestoreProperty("p5")]
    public double TotalProfit50 { get; set; }
    [FirestoreProperty("p6")]
    public double TotalProfit60 { get; set; }
}
