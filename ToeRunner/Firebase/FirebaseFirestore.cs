using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToeRunner.Model.Firebase;
using ToeRunner.Plugin;

namespace ToeRunner.Firebase;

public class FirebaseFirestore : ICloudPlatform {
    private FirestoreDb? _firestoreDb;
    private string _userId = "";
    private const string UserCollection = "user";
    private const string BatchToeRunCollection = "batchToeRun";
    private const string StrategyResultsCollection = "strategyResultReplay";
    private const string TradeResultBackFillCollection = "tradeResultReplay";
    private const int MaxBatchSize = 500;
    
    public async Task<FirestoreDb> Initialize(string projectId, string apiKey, string userId)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", apiKey);
        _firestoreDb = await FirestoreDb.CreateAsync(projectId);
        _userId = userId;
        return _firestoreDb;
    }
    
    public async Task<string> AddBatchToeRun(BatchToeRun batchToeRun)
    {
        if (batchToeRun == null)
            throw new ArgumentNullException(nameof(batchToeRun));
        
        if (_firestoreDb == null)
            throw new InvalidOperationException("FirestoreDb has not been initialized. Call Initialize first.");
            
        // Generate a new GUID for the ID if not already set
        if (string.IsNullOrEmpty(batchToeRun.Id))
            batchToeRun.Id = Guid.NewGuid().ToString();
            
        var docRef = _firestoreDb
            .Collection(UserCollection)
            .Document(_userId)
            .Collection(BatchToeRunCollection)
            .Document(batchToeRun.Id);
        await docRef.CreateAsync(batchToeRun);
        
        return batchToeRun.Id;
    }
    
    public async Task<string> AddStrategyResults(string batchToeRunId, FirebaseStrategyResult strategyResult)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
            
        if (strategyResult == null)
            throw new ArgumentException("Strategy result cannot be null", nameof(strategyResult));
        
        if (_firestoreDb == null)
            throw new InvalidOperationException("FirestoreDb has not been initialized. Call Initialize first.");
            
        // Generate a new GUID for the ID if not already set
        if (string.IsNullOrEmpty(strategyResult.Id))
            strategyResult.Id = Guid.NewGuid().ToString();
            
        var docRef = _firestoreDb
            .Collection(UserCollection)
            .Document(_userId)
            .Collection(BatchToeRunCollection)
            .Document(batchToeRunId)
            .Collection(StrategyResultsCollection)
            .Document(strategyResult.Id);
            
        await docRef.CreateAsync(strategyResult);
        
        return strategyResult.Id;
    }
    
    public async Task UpdateBatchToeRun(string batchToeRunId, long totalStrategies, long uploadedStrategies)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
        
        if (_firestoreDb == null)
            throw new InvalidOperationException("FirestoreDb has not been initialized. Call Initialize first.");
        
        var docRef = _firestoreDb
            .Collection(UserCollection)
            .Document(_userId)
            .Collection(BatchToeRunCollection)
            .Document(batchToeRunId);

        var endTimestamp = new DateTimeConverter().ToFirestore(DateTime.Now);
        
        // Update EndTimestamp, TotalStrategies, and UploadedStrategies fields
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "endTimestamp", endTimestamp },
            { "totalStrategies", totalStrategies },
            { "uploadedStrategies", uploadedStrategies }
        };
        
        await docRef.UpdateAsync(updates);
    }
    
    public async Task AddSegmentStats(string batchToeRunId, string strategyResultId, List<FirebaseSegmentExecutorStats> segmentStats)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
            
        if (string.IsNullOrEmpty(strategyResultId))
            throw new ArgumentException("StrategyResult ID cannot be null or empty", nameof(strategyResultId));
            
        if (segmentStats == null || !segmentStats.Any())
            throw new ArgumentException("Segment stats cannot be null or empty", nameof(segmentStats));
        
        if (_firestoreDb == null)
            throw new InvalidOperationException("FirestoreDb has not been initialized. Call Initialize first.");
            
        // Process segment stats in batches
        for (int i = 0; i < segmentStats.Count; i += MaxBatchSize)
        {
            var batch = _firestoreDb.StartBatch();
            var currentBatch = segmentStats.Skip(i).Take(MaxBatchSize);
            
            foreach (var segmentStat in currentBatch)
            {
                var docRef = _firestoreDb
                    .Collection(UserCollection)
                    .Document(_userId)
                    .Collection(BatchToeRunCollection)
                    .Document(batchToeRunId)
                    .Collection(StrategyResultsCollection)
                    .Document(strategyResultId)
                    .Collection(TradeResultBackFillCollection)
                    .Document(segmentStat.Id);
                    
                batch.Create(docRef, segmentStat);
            }
            
            // Commit the batch
            await batch.CommitAsync();
        }
    }
}