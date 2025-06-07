using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;

/// <summary>
/// Configuration parameters for cryptocurrency sell operations
/// </summary>
[FirestoreData]
public class SellConfig 
{
    [FirestoreProperty("st")]
    public List<SellStrategyConfig> Strategies { get; set; }
}

[FirestoreData]
public class SellStrategyConfig {
    [FirestoreProperty("n")]
    public string Name { get; set; }
    [FirestoreProperty("p", ConverterType = typeof(DynamicToStringConverter))]
    public object Parameters { get; set; }
}
