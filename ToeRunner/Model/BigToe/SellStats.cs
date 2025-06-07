using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.BigToe;

/// <summary>
/// Contains statistics for a completed sell operation
/// </summary>
[FirestoreData]
public class SellStats
{
    /// <summary>
    /// The trading pair used for the sell operation
    /// </summary>
    [FirestoreProperty("tp")]
    public CryptoTradingPair? TradingPair { get; set; }
    
    /// <summary>
    /// The amount of base currency sold
    /// </summary>
    [FirestoreProperty("as")]
    public decimal AmountSold { get; set; }
    
    /// <summary>
    /// The average price received per unit in quote currency
    /// </summary>
    [FirestoreProperty("ap")]
    public decimal AveragePrice { get; set; }
    
    /// <summary>
    /// The total amount received in quote currency
    /// </summary>
    [FirestoreProperty("tr")]
    public decimal TotalReceived { get; set; }
    
    /// <summary>
    /// The total fees paid for the transaction in quote currency
    /// </summary>
    [FirestoreProperty("tf")]
    public decimal TotalFees { get; set; }
    
    /// <summary>
    /// Whether the sell operation was successful
    /// </summary>
    [FirestoreProperty("is")]
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Any error message if the sell operation failed
    /// </summary>
    [FirestoreProperty("em")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The time when the sell operation started
    /// </summary>
    [FirestoreProperty("st", ConverterType = typeof(DateTimeConverter))]
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The time when the sell operation finished
    /// </summary>
    [FirestoreProperty("et", ConverterType = typeof(DateTimeConverter))]
    public DateTime EndTime { get; set; }
}
