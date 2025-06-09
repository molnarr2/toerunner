using Google.Cloud.Firestore;
using ToeRunner.Firebase;
using ToeRunner.Model.BigToe;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Contains statistics for a completed sell operation
/// </summary>
[FirestoreData]
public class FirebaseSellStats
{
    /// <summary>
    /// The amount of base currency sold
    /// </summary>
    [FirestoreProperty("a", ConverterType = typeof(DecimalConverter))]
    public decimal AmountSold { get; set; }
    
    /// <summary>
    /// The total amount received in quote currency
    /// </summary>
    [FirestoreProperty("t", ConverterType = typeof(DecimalConverter))]
    public decimal TotalReceived { get; set; }
    
    /// <summary>
    /// The time when the sell operation finished
    /// </summary>
    [FirestoreProperty("e", ConverterType = typeof(DateTimeConverter))]
    public DateTime EndTime { get; set; }
}
