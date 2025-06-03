namespace ToeRunner.Model.BigToe;

/// <summary>
/// Data structure for storing all segment stats for a specific executor
/// </summary>
public class ExecutorEvaluationResult {
    /// <summary>
    /// The name of the executor
    /// </summary>
    public string ExecutorName { get; set; }
    
    /// <summary>
    /// List of stats for each segment run
    /// </summary>
    public List<SegmentExecutorStats> SegmentStats { get; set; } = new List<SegmentExecutorStats>();
    
    /// <summary>
    /// Creates a new instance of ExecutorEvaluationResult
    /// </summary>
    /// <param name="executorName">The name of the executor</param>
    public ExecutorEvaluationResult(string executorName) {
        ExecutorName = executorName;
    }
    
    /// <summary>
    /// Adds segment stats to the list
    /// </summary>
    /// <param name="segmentStats">The segment stats to add</param>
    public void AddSegmentStats(SegmentExecutorStats segmentStats) {
        SegmentStats.Add(segmentStats);
    }
}
