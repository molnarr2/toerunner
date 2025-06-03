namespace ToeRunner.Model.BigToe;

public class TradeContainerConfig {
    public string Name { get; set; }
    public List<OrchestratorContainerConfig> Orchestrators { get; set; }
    public List<ExecutorContainerConfig> Executors { get; set; }
}
