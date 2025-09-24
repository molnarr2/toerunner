namespace ToeRunner.Model;

public enum FilterPercentageType {
    p00,
    p08,
    p10,
    p15,
    p20,
    p25,
    p30,
    p35,
    p40,
    p50,
    p60
}

public class ToeRunnerConfig {
    public string Name {get; set;}
    public string UserId { get; set; }
    public string Server { get; set; }
    public int ParallelRunners { get; set; }
    public string? WorkspacePath { get; set; }
    public string? BigToeExecutablePath { get; set; }
    public string? TinyToeExecutablePath { get; set; }
    public List<RunConfig>? Runs { get; set; }
    public FirebaseConfig? Firebase { get; set; }
    /// <summary>
    /// The percentage of successful strategies to upload. 
    /// 1.0 = 100%
    /// </summary>
    public decimal UploadStrategyPercentage { get; set; }
    
    /// <summary>
    /// To determine if a strategy should be uploaded based on profit percentage.
    /// </summary>
    public FilterPercentageType FilterProfitPercentage { get; set; } 
}

public class FirebaseConfig {
    public bool UseMock { get; set; }
    public string? ProjectId { get; set; }
    public string? ApiKey { get; set; }
}

public class RunConfig {
    public string? Name { get; set; }
    public int RunCount { get; set; }
    public string? BigToeEnvironmentConfigPath { get; set; }
    public List<string>? TinyToeConfigPaths { get; set; } 
}

public class ToeJob {
    public string RunName { get; set; }
    public string? Name { get; set; }
    public string? BigToeEnvironmentConfigPath { get; set; }
    public string? TinyToeConfigPath { get; set; }
}
