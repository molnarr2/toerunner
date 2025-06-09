using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class ExecutorContainerConfig {
    [FirestoreProperty("name")]
    public string Name { get; set; }
    [FirestoreProperty("executor")]
    public ScheduledTradeExecutorConfig Executor { get; set; }
    [FirestoreProperty("buy")]
    public BuyConfig? Buy { get; set; }
    [FirestoreProperty("sell")]
    public SellConfig? Sell { get; set; }
}
