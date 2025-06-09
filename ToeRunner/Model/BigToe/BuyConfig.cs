using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;

/// <summary>
/// Configuration parameters for cryptocurrency buy operations
/// </summary>
[FirestoreData]
public class BuyConfig 
{
    /// <summary>
    /// The amount to buy in base currency units
    /// </summary>
    [FirestoreProperty("amount", ConverterType = typeof(DecimalConverter))]
    public decimal Amount { get; set; }
 
    [FirestoreProperty("strategies")]
    public List<BuyStrategyConfig> Strategies { get; set; }
}

[FirestoreData]
public class BuyStrategyConfig {
    [FirestoreProperty("name")]
    public string Name { get; set; }
    [FirestoreProperty("parameters", ConverterType = typeof(DynamicToStringConverter))]
    public object Parameters { get; set; }
}
