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
    private const string StrategyResultsCollection = "strategyResult";
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
    
    public async Task AddStrategyResults(string batchToeRunId, List<FirebaseStrategyResult> strategyResults)
    {
        if (string.IsNullOrEmpty(batchToeRunId))
            throw new ArgumentException("BatchToeRun ID cannot be null or empty", nameof(batchToeRunId));
            
        if (strategyResults == null || !strategyResults.Any())
            throw new ArgumentException("Strategy results cannot be null or empty", nameof(strategyResults));
        
        if (_firestoreDb == null)
            throw new InvalidOperationException("FirestoreDb has not been initialized. Call Initialize first.");
            
        // Process strategy results in batches
        for (int i = 0; i < strategyResults.Count; i += MaxBatchSize)
        {
            var batch = _firestoreDb.StartBatch();
            var currentBatch = strategyResults.Skip(i).Take(MaxBatchSize);
            
            foreach (var result in currentBatch)
            {
                // Generate a new GUID for the ID if not already set
                if (string.IsNullOrEmpty(result.Id))
                    result.Id = Guid.NewGuid().ToString();
                    
                var docRef = _firestoreDb
                    .Collection(UserCollection)
                    .Document(_userId)
                    .Collection(BatchToeRunCollection)
                    .Document(batchToeRunId)
                    .Collection(StrategyResultsCollection)
                    .Document(result.Id);
                    
                batch.Create(docRef, result);
            }
            
            // Commit the batch
            await batch.CommitAsync();
        }
    }
    
    public async Task UpdateBatchToeRun(string batchToeRunId, long totalStrategies)
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
        
        // Update only the EndTimestamp and TotalStrategies fields
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "endTimestamp", DateTime.Now },
            { "totalStrategies", totalStrategies }
        };
        
        await docRef.UpdateAsync(updates);
    }
}