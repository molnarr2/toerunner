using Google.Cloud.Firestore;

namespace ToeRunner.Model.BigToe;

[FirestoreData]
public class PlaybackSegmentDetails {
    [FirestoreProperty("id")]
    public string Id { get; set; }
    [FirestoreProperty("filepath")]
    public string FilePath { get; set; }
    [FirestoreProperty("useDex")]
    public Boolean UseDex { get; set; }
    [FirestoreProperty("cryptoTradingPair")]
    public CryptoTradingPair CryptoTradingPair { get; set; }
}