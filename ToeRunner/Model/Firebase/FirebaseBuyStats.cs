using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Contains statistics for a completed buy operation
/// </summary>
[FirestoreData]
public class FirebaseBuyStats
{
    /// <summary>
    /// The amount of base currency bought
    /// </summary>
    [FirestoreProperty("amountBought", ConverterType = typeof(DecimalConverter))]
    public decimal AmountBought { get; set; }
    
    /// <summary>
    /// The total cost in quote currency
    /// </summary>
    [FirestoreProperty("totalCost", ConverterType = typeof(DecimalConverter))]
    public decimal TotalCost { get; set; }

    /// <summary>
    /// The time when the buy operation finished
    /// </summary>
    [FirestoreProperty("endTime", ConverterType = typeof(DateTimeConverter))]
    public DateTime EndTime { get; set; }
}
