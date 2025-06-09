using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class ScheduledTradeExecutorConfig 
{
    [FirestoreProperty("strategies")]
    public required List<ExecutorStrategyConfig> Strategies { get; set; }
}

[FirestoreData]
public class ExecutorStrategyConfig {
    [FirestoreProperty("name")]
    public string Name { get; set; }
    [FirestoreProperty("parameters", ConverterType = typeof(DynamicToStringConverter))]
    public object Parameters { get; set; }
}
