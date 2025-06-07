using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class ScheduledTradeExecutorConfig 
{
    [FirestoreProperty("st")]
    public required List<ExecutorStrategyConfig> Strategies { get; set; }
}

[FirestoreData]
public class ExecutorStrategyConfig {
    [FirestoreProperty("n")]
    public string Name { get; set; }
    [FirestoreProperty("p", ConverterType = typeof(DynamicToStringConverter))]
    public object Parameters { get; set; }
}
