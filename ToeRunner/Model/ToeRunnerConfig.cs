namespace ToeRunner.Model;

public class ToeRunnerConfig {
    public int ParallelRunners { get; set; }
    public string BigToeExecutablePath { get; set; }
    public string TinyToeExecutablePath { get; set; }
    public List<RunConfig> Runs { get; set; }
}

public class RunConfig {
    public string Name { get; set; }
    public int RunCount { get; set; }
    public string BigToeEnvironmentConfigPath { get; set; }
    public List<string> TinyToeConfigPaths { get; set; } 
}

public class ToeJob {
    public string Name { get; set; }
    public string BigToeEnvironmentConfigPath { get; set; }
    public string TinyToeConfigPath { get; set; }
}
