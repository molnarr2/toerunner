using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToeRunner.Model.Firebase;
using ToeRunner.Plugin;

namespace ToeRunner.Firebase;

public class MockCloudPlatform : ICloudPlatform
{
    private Dictionary<string, BatchToeRun> _batchToeRuns = new Dictionary<string, BatchToeRun>();
    private Dictionary<string, List<FirebaseStrategyResult>> _strategyResults = new Dictionary<string, List<FirebaseStrategyResult>>();
    
    public Task<FirestoreDb> Initialize(string projectId, string apiKey, string userId)
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
    
    public Task<string> AddStrategyResults(string batchToeRunId, FirebaseStrategyResult strategyResult)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
            
        if (strategyResult == null)
            throw new ArgumentException("Strategy result cannot be null", nameof(strategyResult));
            
        if (!_strategyResults.ContainsKey(batchToeRunId))
            _strategyResults[batchToeRunId] = new List<FirebaseStrategyResult>();
            
        // Generate a new GUID for the ID if not already set
        if (string.IsNullOrEmpty(strategyResult.Id))
            strategyResult.Id = Guid.NewGuid().ToString();
            
        _strategyResults[batchToeRunId].Add(strategyResult);
        
        Console.WriteLine($"[MOCK] Added strategy result {strategyResult.Id} for BatchToeRun ID: {batchToeRunId}");
        
        return Task.FromResult(strategyResult.Id);
    }
    
    public Task AddSegmentStats(string batchToeRunId, string strategyResultId, List<FirebaseSegmentExecutorStats> segmentStats)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
            
        if (string.IsNullOrEmpty(strategyResultId))
            throw new ArgumentException("StrategyResult ID cannot be null or empty", nameof(strategyResultId));
            
        if (segmentStats == null || segmentStats.Count == 0)
            throw new ArgumentException("Segment stats cannot be null or empty", nameof(segmentStats));
        
        // Filter out segments with no trades
        var segmentsWithTrades = segmentStats.Where(s => s.TotalTrades > 0).ToList();
        
        if (!segmentsWithTrades.Any())
        {
            Console.WriteLine($"[MOCK] No segments with trades to upload for strategy {strategyResultId}");
            return Task.CompletedTask;
        }
            
        Console.WriteLine($"[MOCK] Added {segmentsWithTrades.Count} segment stats (filtered from {segmentStats.Count}) for strategy {strategyResultId} in batch {batchToeRunId}");
        
        return Task.CompletedTask;
    }

    public Task UpdateBatchToeRun(string batchToeRunId, long totalStrategies, long uploadedStrategies) {
        Console.WriteLine($"[MOCK] Update BatchtoeRun totalStrategies: {totalStrategies} BatchToeRun ID: {batchToeRunId}");
        return Task.CompletedTask;
    }
}
