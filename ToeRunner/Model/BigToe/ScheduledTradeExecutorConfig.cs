using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class ScheduledTradeExecutorConfig 
{
    [FirestoreProperty("st")]
    public List<ExecutorStrategyConfig> Strategies { get; set; }
}

[FirestoreData]
public class ExecutorStrategyConfig {
    [FirestoreProperty("n")]
    public string Name { get; set; }
    [FirestoreProperty("p")]
    public dynamic Parameters { get; set; }
}
