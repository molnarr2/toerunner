namespace ToeRunner.Model.BigToe;

public class ScheduledTradeOrchestratorConfig {
    public List<OrchestratorStrategyConfig> Strategies { get; set; }
}

public class OrchestratorStrategyConfig {
    public string Name { get; set; }
    public dynamic Parameters { get; set; }
}
