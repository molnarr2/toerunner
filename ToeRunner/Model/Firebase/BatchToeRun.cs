using ToeRunner.Model.BigToe;
using Google.Cloud.Firestore;
using ToeRunner.Firebase;

namespace ToeRunner.Model.Firebase;

[FirestoreData]
public class BatchToeRun {
    [FirestoreProperty("id")]
    public String Id { get; set; }
    [FirestoreProperty("name")]
    public String Name { get; set; }
    [FirestoreProperty("description")]
    public String Description { get; set; }
    [FirestoreProperty("server")]
    public String Server { get; set; }
    [FirestoreProperty("startTimestamp", ConverterType = typeof(DateTimeConverter))]
    public DateTime StartTimestamp { get; set; }
    [FirestoreProperty("endTimestamp", ConverterType = typeof(DateTimeConverter))]
    public DateTime EndTimestamp { get; set; }
    [FirestoreProperty("segmentDetails")]
    public List<PlaybackSegmentDetails> SegmentDetails { get; set; }
}
