using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Information about a segment's training status
/// </summary>
[FirestoreData]
public class SegmentTrainInfo
{
    /// <summary>
    /// Unique identifier for the segment
    /// </summary>
    [FirestoreProperty("segmentId")]
    public string SegmentId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this segment is used for training
    /// </summary>
    [FirestoreProperty("trainOn")]
    public bool TrainOn { get; set; }
}

[FirestoreData]
public class BatchToeRun {
    [FirestoreProperty("id")]
    public string Id { get; set; }
    [FirestoreProperty("name")] 
    public string Name { get; set; }
    [FirestoreProperty("parallelRunners")] 
    public int ParallelRunners { get; set; }
    [FirestoreProperty("server")]
    public string Server { get; set; }
    [FirestoreProperty("startTimestamp", ConverterType = typeof(DateTimeConverter))]
    public DateTime StartTimestamp { get; set; }
    
    [FirestoreProperty("endTimestamp", ConverterType = typeof(DateTimeConverter))]
    public DateTime EndTimestamp { get; set; }
    
    [FirestoreProperty("jobCount")]
    public int JobCount { get; set; }
    
    [FirestoreProperty("totalStrategies")]
    public long TotalStrategies { get; set; }
    
    [FirestoreProperty("uploadedStrategies")]
    public long UploadedStrategies { get; set; }
    
    [FirestoreProperty("segmentTrainInfo")]
    public List<SegmentTrainInfo> SegmentTrainInfo { get; set; } = new();
    
    [FirestoreProperty("segmentCount")]
    public int SegmentCount { get; set; }
}
