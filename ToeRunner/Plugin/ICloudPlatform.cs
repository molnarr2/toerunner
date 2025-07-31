using Google.Cloud.Firestore;
using ToeRunner.Model.Firebase;

namespace ToeRunner.Plugin;

public interface ICloudPlatform {
    Task<FirestoreDb> Initialize(string projectId, string apiKey, string userId);
    
    Task<string> AddBatchToeRun(BatchToeRun batchToeRun);
    
    Task<string> AddStrategyResults(string batchToeRunId, FirebaseStrategyResult strategyResult);
    
    Task AddSegmentStats(string batchToeRunId, string strategyResultId, List<FirebaseSegmentExecutorStats> segmentStats);
    
    Task UpdateBatchToeRun(string batchToeRunId, long totalStrategies, long uploadedStrategies);
}
