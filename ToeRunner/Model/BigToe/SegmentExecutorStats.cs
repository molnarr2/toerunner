using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

/// <summary>
/// Data structure for storing executor stats for a specific segment
/// </summary>
[FirestoreData]
public class SegmentExecutorStats {
    /// <summary>
    /// The segment number (0-based)
    /// </summary>
    [FirestoreProperty("sn")]
    public int SegmentNumber { get; set; }
    
    /// <summary>
    /// The executor stats for this segment
    /// </summary>
    [FirestoreProperty("es")]
    public ScheduledTradeExecutorStats ExecutorStats { get; set; }
}