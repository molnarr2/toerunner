using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;

/// <summary>
/// Contains statistics for a completed buy operation
/// </summary>
[FirestoreData]
public class BuyStats
{
    /// <summary>
    /// The trading pair used for the buy operation
    /// </summary>
    [FirestoreProperty("tp")]
    public CryptoTradingPair? TradingPair { get; set; }
    
    /// <summary>
    /// The amount of base currency bought
    /// </summary>
    [FirestoreProperty("ab", ConverterType = typeof(DecimalConverter))]
    public decimal AmountBought { get; set; }
    
    /// <summary>
    /// The average price paid per unit in quote currency
    /// </summary>
    [FirestoreProperty("ap", ConverterType = typeof(DecimalConverter))]
    public decimal AveragePrice { get; set; }
    
    /// <summary>
    /// The total cost in quote currency
    /// </summary>
    [FirestoreProperty("tc", ConverterType = typeof(DecimalConverter))]
    public decimal TotalCost { get; set; }
    
    /// <summary>
    /// The total fees paid for the transaction in quote currency
    /// </summary>
    [FirestoreProperty("tf", ConverterType = typeof(DecimalConverter))]
    public decimal TotalFees { get; set; }
    
    /// <summary>
    /// Whether the buy operation was successful
    /// </summary>
    [FirestoreProperty("is")]
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Any error message if the buy operation failed
    /// </summary>
    [FirestoreProperty("em")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The time when the buy operation started
    /// </summary>
    [FirestoreProperty("st", ConverterType = typeof(DateTimeConverter))]
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The time when the buy operation finished
    /// </summary>
    [FirestoreProperty("et", ConverterType = typeof(DateTimeConverter))]
    public DateTime EndTime { get; set; }
}
