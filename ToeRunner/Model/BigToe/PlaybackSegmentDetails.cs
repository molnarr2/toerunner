using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class PlaybackSegmentDetails {
    [FirestoreProperty("id")]
    public string Id { get; set; }
    [FirestoreProperty("fp")]
    public string FilePath { get; set; }
    [FirestoreProperty("ud")]
    public Boolean UseDex { get; set; }
    [FirestoreProperty("cp")]
    public CryptoTradingPair CryptoTradingPair { get; set; }
}