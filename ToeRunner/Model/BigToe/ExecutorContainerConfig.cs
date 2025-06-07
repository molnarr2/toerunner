using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class ExecutorContainerConfig {
    [FirestoreProperty("n")]
    public string Name { get; set; }
    [FirestoreProperty("e")]
    public ScheduledTradeExecutorConfig Executor { get; set; }
    [FirestoreProperty("b")]
    public BuyConfig? Buy { get; set; }
    [FirestoreProperty("s")]
    public SellConfig? Sell { get; set; }
}
