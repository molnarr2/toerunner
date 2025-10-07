namespace ToeRunner.Model;

public enum FilterPercentageType {
    p00,
    p001,
    p08,
    p10,
    p15,
    p20,
    p25
}

public class ToeRunnerConfig {
    public required string Name {get; set;}
    public required string UserId { get; set; }
    public required string Server { get; set; }
    public int ParallelRunners { get; set; }
    public string? WorkspacePath { get; set; }
    public required string BigToeExecutablePath { get; set; }
    public required string TinyToeExecutablePath { get; set; }
    public List<string>? TinyToeConfigPaths { get; set; }
    public required int PerFileRunCount { get; set; }
    public required int TinyToeRunCount { get; set; }
    public required string BigToeEnvironmentConfigPath { get; set; }
    public required string BigToeSegmentPath { get; set; }
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

public class ToeJob {
    public required string RunName { get; set; }
    public string? Name { get; set; }
    public string? TinyToeConfigPath { get; set; }
}
