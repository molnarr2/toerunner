namespace ToeRunner.Model.BigToe;

/// <summary>
/// Contains statistics for a completed buy operation
/// </summary>
public class BuyStats
{
    /// <summary>
    /// The trading pair used for the buy operation
    /// </summary>
    public CryptoTradingPair? TradingPair { get; set; }
    
    /// <summary>
    /// The amount of base currency bought
    /// </summary>
    public decimal AmountBought { get; set; }
    
    /// <summary>
    /// The average price paid per unit in quote currency
    /// </summary>
    public decimal AveragePrice { get; set; }
    
    /// <summary>
    /// The total cost in quote currency
    /// </summary>
    public decimal TotalCost { get; set; }
    
    /// <summary>
    /// The total fees paid for the transaction in quote currency
    /// </summary>
    public decimal TotalFees { get; set; }
    
    /// <summary>
    /// Whether the buy operation was successful
    /// </summary>
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Any error message if the buy operation failed
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The time when the buy operation started
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// The time when the buy operation finished
    /// </summary>
    public DateTime EndTime { get; set; }
}
