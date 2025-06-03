namespace ToeRunner.Model.BigToe;

/// <summary>
/// Data structure for storing executor stats for a specific segment
/// </summary>
public class SegmentExecutorStats {
    /// <summary>
    /// The segment number (0-based)
    /// </summary>
    public int SegmentNumber { get; set; }
    
    /// <summary>
    /// The executor stats for this segment
    /// </summary>
    public ScheduledTradeExecutorStats ExecutorStats { get; set; }
    
    /// <summary>
    /// Creates a new instance of SegmentExecutorStats
    /// </summary>
    /// <param name="segmentNumber">The segment number</param>
    /// <param name="executorStats">The executor stats</param>
    public SegmentExecutorStats(int segmentNumber, ScheduledTradeExecutorStats executorStats) {
        SegmentNumber = segmentNumber;
        ExecutorStats = executorStats;
    }
}