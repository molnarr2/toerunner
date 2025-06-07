using Google.Cloud.Firestore;

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
    [FirestoreProperty("a")]
    public decimal Amount { get; set; }
 
    [FirestoreProperty("st")]
    public List<BuyStrategyConfig> Strategies { get; set; }
}

[FirestoreData]
public class BuyStrategyConfig {
    [FirestoreProperty("n")]
    public string Name { get; set; }
    [FirestoreProperty("p")]
    public dynamic Parameters { get; set; }
}
