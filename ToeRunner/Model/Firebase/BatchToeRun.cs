using ToeRunner.Model.BigToe;

namespace ToeRunner.Model.Firebase;

public class BatchToeRun {
    public String Id { get; set; }
    public String Name { get; set; }
    public String Description { get; set; }
    public String Server { get; set; }
    public DateTime StartTimestamp { get; set; }
    public DateTime EndTimestamp { get; set; }
    public List<PlaybackSegmentDetails> SegmentDetails { get; set; }
}
