using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

/// <summary>
/// Data structure for storing all segment stats for a specific executor
/// </summary>
[FirestoreData]
public class ExecutorEvaluationResult {
    /// <summary>
    /// The name of the executor
    /// </summary>
    [FirestoreProperty("en")]
    public string ExecutorName { get; set; }
    
    /// <summary>
    /// List of stats for each segment run
    /// </summary>
    [FirestoreProperty("ss")]
    public List<SegmentExecutorStats> SegmentStats { get; set; } = new List<SegmentExecutorStats>();
}
