namespace ToeRunner.Model.BigToe;

public class ScheduledTradeExecutorConfig 
{
    public List<ExecutorStrategyConfig> Strategies { get; set; }
}

public class ExecutorStrategyConfig {
    public string Name { get; set; }
    public dynamic Parameters { get; set; }
}
