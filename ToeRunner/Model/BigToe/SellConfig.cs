namespace ToeRunner.Model.BigToe;

/// <summary>
/// Configuration parameters for cryptocurrency sell operations
/// </summary>
public class SellConfig 
{
    public List<SellStrategyConfig> Strategies { get; set; }
}

public class SellStrategyConfig {
    public string Name { get; set; }
    public dynamic Parameters { get; set; }
}
