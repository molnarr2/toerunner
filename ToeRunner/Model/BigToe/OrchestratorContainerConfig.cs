namespace ToeRunner.Model.BigToe;

public class OrchestratorContainerConfig {
    public string Name { get; set; }
    public ScheduledTradeOrchestratorConfig Orchestrator { get; set; }
    public List<string> Executors { get; set; }
}
