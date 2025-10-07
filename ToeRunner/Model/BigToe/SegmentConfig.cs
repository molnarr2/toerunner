namespace ToeRunner.Model.BigToe;

/// <summary>
/// Configuration for playback segments containing data type and individual segment information
/// </summary>
public class SegmentConfig
{
    /// <summary>
    /// The type of data for the segments (e.g., "Jupiter")
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// List of individual segments to be played back
    /// </summary>
    public List<SegmentInfo> Segments { get; set; } = new();
}

/// <summary>
/// Information about an individual playback segment
/// </summary>
public class SegmentInfo
{
    /// <summary>
    /// Unique identifier for the segment
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// File path to the segment data
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Asset ID for the trading pair
    /// </summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>
    /// Quote Asset ID for the trading pair
    /// </summary>
    public string QuoteAssetId { get; set; } = string.Empty;

    /// <summary>
    /// If the segment is being used for training purposes (i.e., not used for testing)
    /// This is used more in the ToeRunner and PinkyToe to train on and then use the testing segments to see how well it performs.
    /// ToeRunner is looking for successful strategies runs on the training portion and then using the testing portion to see how well it performs.
    /// </summary>
    public bool TrainOn { get; set; } = true;
}
