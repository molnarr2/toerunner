using Google.Cloud.Firestore;

namespace ToeRunner.Model.Firebase;

/// <summary>
/// Validation results comparing test vs validation performance
/// </summary>
[FirestoreData]
public class FirebaseStrategyValidation
{
    [FirestoreProperty("testPerformance")]
    public FirebaseStrategyPerformance TestPerformance { get; set; } = new();
    
    [FirestoreProperty("validationPerformance")]
    public FirebaseStrategyPerformance ValidationPerformance { get; set; } = new();
    
    [FirestoreProperty("qualityScore")]
    public double QualityScore { get; set; }
    
    [FirestoreProperty("consistencyScore")]
    public double ConsistencyScore { get; set; }
    
    [FirestoreProperty("notes")]
    public List<string> ValidationNotes { get; set; } = new();
}
