namespace ToeRunner.Model.BigToe;

public class ExecutorContainerConfig {
    public string Name { get; set; }
    public ScheduledTradeExecutorConfig Executor { get; set; }
    public BuyConfig? Buy { get; set; }
    public SellConfig? Sell { get; set; }
}
