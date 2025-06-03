using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToeRunner.Model.Firebase;

namespace ToeRunner.Plugin;

public interface ICloudPlatform {
    Task<FirestoreDb> Initialize(string projectId, string apiKey);
    
    Task<string> AddBatchToeRun(BatchToeRun batchToeRun);
    
    Task AddStrategyResults(string batchToeRunId, List<StrategyResult> strategyResults);
    
    Task SaveBatchToeRun(BatchToeRun batchToeRun);
}
