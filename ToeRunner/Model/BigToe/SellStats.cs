namespace ToeRunner.Model.BigToe;

/// <summary>
/// Contains statistics for a completed sell operation
/// </summary>
public class SellStats
{
    /// <summary>
    /// The trading pair used for the sell operation
    /// </summary>
    public CryptoTradingPair? TradingPair { get; set; }
    
    /// <summary>
    /// The amount of base currency sold
    /// </summary>
    public decimal AmountSold { get; set; }
    
    /// <summary>
    /// The average price received per unit in quote currency
    /// </summary>
    public decimal AveragePrice { get; set; }
    
    /// <summary>
    /// The total amount received in quote currency
    /// </summary>
    public decimal TotalReceived { get; set; }
    
    /// <summary>
    /// The total fees paid for the transaction in quote currency
    /// </summary>
    public decimal TotalFees { get; set; }
    
    /// <summary>
    /// Whether the sell operation was successful
    /// </summary>
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Any error message if the sell operation failed
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The time when the sell operation started
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The time when the sell operation finished
    /// </summary>
    public DateTime EndTime { get; set; }
}
