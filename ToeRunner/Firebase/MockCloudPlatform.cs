using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToeRunner.Model.Firebase;
using ToeRunner.Plugin;

namespace ToeRunner.Firebase;

public class MockCloudPlatform : ICloudPlatform
{
    private Dictionary<string, BatchToeRun> _batchToeRuns = new Dictionary<string, BatchToeRun>();
    private Dictionary<string, List<FirebaseStrategyResult>> _strategyResults = new Dictionary<string, List<FirebaseStrategyResult>>();
    
    public Task<FirestoreDb> Initialize(string projectId, string apiKey)
    {
        Console.WriteLine($"[MOCK] Initialized cloud platform with Project ID: {projectId}");
        // We can't return a real FirestoreDb in the mock, but the interface requires a non-null return
        // This is a workaround for the mock implementation - in a real scenario we would refactor the interface
        throw new NotImplementedException("Mock implementation does not provide a FirestoreDb instance");
    }
    
    public Task<string> AddBatchToeRun(BatchToeRun batchToeRun)
    {
        if (batchToeRun == null)
            throw new ArgumentNullException(nameof(batchToeRun));
            
        // Generate a new GUID for the ID if not already set
        if (string.IsNullOrEmpty(batchToeRun.Id))
            batchToeRun.Id = Guid.NewGuid().ToString();
            
        _batchToeRuns[batchToeRun.Id] = batchToeRun;
        Console.WriteLine($"[MOCK] Added BatchToeRun with ID: {batchToeRun.Id}");
        
        return Task.FromResult(batchToeRun.Id);
    }
    
    public Task AddStrategyResults(string batchToeRunId, List<FirebaseStrategyResult> strategyResults)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
            
        if (strategyResults == null || strategyResults.Count == 0)
            throw new ArgumentException("Strategy results cannot be null or empty", nameof(strategyResults));
            
        if (!_strategyResults.ContainsKey(batchToeRunId))
            _strategyResults[batchToeRunId] = new List<FirebaseStrategyResult>();
            
        foreach (var result in strategyResults)
        {
            // Generate a new GUID for the ID if not already set
            if (string.IsNullOrEmpty(result.Id))
                result.Id = Guid.NewGuid().ToString();
                
            _strategyResults[batchToeRunId].Add(result);
        }
        
        Console.WriteLine($"[MOCK] Added {strategyResults.Count} strategy results for BatchToeRun ID: {batchToeRunId}");
        
        return Task.CompletedTask;
    }
    
    public Task SaveBatchToeRun(BatchToeRun batchToeRun)
    {
        if (batchToeRun == null)
            throw new ArgumentNullException(nameof(batchToeRun));
            
        if (string.IsNullOrEmpty(batchToeRun.Id))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty when updating", nameof(batchToeRun.Id));
            
        _batchToeRuns[batchToeRun.Id] = batchToeRun;
        Console.WriteLine($"[MOCK] Updated BatchToeRun with ID: {batchToeRun.Id}");
        
        return Task.CompletedTask;
    }
}
