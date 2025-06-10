using Google.Cloud.Firestore;
using ToeRunner.Model.Firebase;

namespace ToeRunner.Plugin;

public interface ICloudPlatform {
    Task<FirestoreDb> Initialize(string projectId, string apiKey, string userId);
    
    Task<string> AddBatchToeRun(BatchToeRun batchToeRun);
    
    Task AddStrategyResults(string batchToeRunId, List<FirebaseStrategyResult> strategyResults);
    
    Task UpdateBatchToeRun(string batchToeRunId, long totalStrategies);
}
