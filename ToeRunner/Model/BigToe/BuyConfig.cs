namespace ToeRunner.Model.BigToe;

/// <summary>
/// Configuration parameters for cryptocurrency buy operations
/// </summary>
public class BuyConfig 
{
    /// <summary>
    /// The amount to buy in base currency units
    /// </summary>
    public decimal Amount { get; set; }
 
    public List<BuyStrategyConfig> Strategies { get; set; }
}

public class BuyStrategyConfig {
    public string Name { get; set; }
    public dynamic Parameters { get; set; }
}
