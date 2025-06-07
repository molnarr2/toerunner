using ToeRunner.Model.BigToe;
using Google.Cloud.Firestore;

namespace ToeRunner.Model.Firebase;

[FirestoreData]
public class BatchToeRun {
    [FirestoreProperty("id")]
    public String Id { get; set; }
    [FirestoreProperty("n")]
    public String Name { get; set; }
    [FirestoreProperty("d")]
    public String Description { get; set; }
    [FirestoreProperty("s")]
    public String Server { get; set; }
    [FirestoreProperty("st")]
    public DateTime StartTimestamp { get; set; }
    [FirestoreProperty("et")]
    public DateTime EndTimestamp { get; set; }
    [FirestoreProperty("sd")]
    public List<PlaybackSegmentDetails> SegmentDetails { get; set; }
}
