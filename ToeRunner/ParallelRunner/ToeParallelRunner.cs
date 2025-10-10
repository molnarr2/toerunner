using System;
using System.Collections.Concurrent;
using System.Text.Json;
using ToeRunner.Conversion;
using ToeRunner.Model;
using ToeRunner.Model.BigToe;
using ToeRunner.Model.Firebase;
using ToeRunner.Plugin;
using ToeRunner.StrategyAnalysis;
using ToeRunner.ToeRun;

namespace ToeRunner.ParallelRunner;

/// <summary>
/// Handles parallel execution of ToeJobs using multiple threads.
/// </summary>
public class ToeParallelRunner
{
    private readonly ToeRunnerConfig _config;
    private readonly IToeRunFactory _toeRunFactory;
    private readonly ICloudPlatform? _cloudPlatform;
    private readonly SegmentConfig? _segmentConfig;
    private readonly StrategyAnalysisService _strategyAnalysisService;
    private int _totalStrategiesProcessed = 0;
    private int _totalUploadedStrategies = 0;
    private int _jobRuns = 0;
    private readonly object _lockObject = new object();
    private const int STRATEGY_UPDATE_THRESHOLD = 10;
    
    /// <summary>
    /// Initializes a new instance of the ToeParallelRunner class.
    /// </summary>
    /// <param name="config">The configuration for the ToeRunner.</param>
    /// <param name="toeRunFactory">The factory for creating IToeRun instances.</param>
    /// <param name="cloudPlatform">Optional cloud platform for data storage, can be null.</param>
    public ToeParallelRunner(ToeRunnerConfig config, IToeRunFactory toeRunFactory, ICloudPlatform? cloudPlatform)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _toeRunFactory = toeRunFactory ?? throw new ArgumentNullException(nameof(toeRunFactory));
        _cloudPlatform = cloudPlatform;
        _segmentConfig = LoadSegmentConfig();
        _strategyAnalysisService = new StrategyAnalysisService(StrategyAnalysisType.MCDA);
    }
    
    /// <summary>
    /// Processes a list of ToeJobs in parallel using multiple threads.
    /// </summary>
    /// <param name="jobs">The list of ToeJobs to process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessJobsAsync(List<ToeJob> jobs)
    {
        if (jobs == null || !jobs.Any())
        {
            Console.WriteLine("No jobs to process.");
            return;
        }
        PrintJobList(jobs);
        
        // Convert the list to a thread-safe queue
        var jobQueue = new ConcurrentQueue<ToeJob>(jobs);
        
        Console.WriteLine($"Starting parallel processing of {jobs.Count} jobs with {_config.ParallelRunners} threads...");
        
        // Create a batch record if cloud platform is available
        var batchId = await CreateBatchRecord(jobQueue.Count);

        // Create tasks for each thread
        var tasks = new List<Task>();
        for (int i = 0; i < _config.ParallelRunners; i++)
        {
            var threadId = i + 1;
            tasks.Add(Task.Run(async () => await ProcessJobsThread(jobQueue, threadId, batchId)));
        }
        
        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
        
        // Upload strategies at the end
        await UploadTopStrategiesAsync(batchId);
        
        Console.WriteLine($"Updated batch record {batchId} with total strategies: {_totalStrategiesProcessed}, uploaded strategies: {_totalUploadedStrategies}");
        await UpdateBatchToeRunAsync(batchId, _totalStrategiesProcessed, _totalUploadedStrategies);
        
        Console.WriteLine("All jobs have been processed successfully!");
    }
    
    /// <summary>
    /// Thread worker method that processes jobs from the queue.
    /// </summary>
    /// <param name="jobQueue">The thread-safe queue of jobs.</param>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="batchId">The ID of the batch run, can be null.</param>
    private async Task ProcessJobsThread(ConcurrentQueue<ToeJob> jobQueue, int threadId, string? batchId)
    {
        while (jobQueue.TryDequeue(out var job))
        {
            await ProcessJob(job, threadId, batchId);
        }
        
        Console.WriteLine($"Thread {threadId} completed - no more jobs to process.");
    }
    
    /// <summary>
    /// Processes a single job.
    /// </summary>
    /// <param name="job">The job to process.</param>
    /// <param name="threadId">The ID of the thread processing the job.</param>
    /// <param name="batchId">The ID of the batch run, can be null.</param>
    private async Task ProcessJob(ToeJob job, int threadId, string? batchId)
    {
        Console.WriteLine($"Thread {threadId} processing job: {job.Name} (BigToe: {_config.BigToeEnvironmentConfigPath}, TinyToe: {job.TinyToeConfigPath})");
        
        try
        {
            // Create an IToeRun instance using the factory
            IToeRun toeRun = _toeRunFactory.Create(job, threadId, batchId, _segmentConfig, _strategyAnalysisService);
            
            // Run the IToeRun instance
            await toeRun.RunAsync();
            
            // Update the total strategies count
            int strategyCount = toeRun.GetStrategyCount();
            lock (_lockObject)
            {
                _totalStrategiesProcessed += strategyCount;
            }
            
            Console.WriteLine($"Thread {threadId} completed job: {job.Name} with {strategyCount} strategies. Total strategies processed: {_totalStrategiesProcessed}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Thread {threadId} failed to process job {job.Name}: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Creates a batch record in the cloud platform.
    /// </summary>
    /// <returns>The ID of the created batch record, or null if creation failed.</returns>
    private async Task<string?> CreateBatchRecord(int jobQueueCount)
    {
        if (_cloudPlatform == null)
        {
            Console.WriteLine("No cloud platform available, skipping batch record creation.");
            return null;
        }
        
        try
        {
            var batchToeRun = new BatchToeRun
            {
                Id = Guid.NewGuid().ToString(),
                Name = _config.Name,
                ParallelRunners = _config.ParallelRunners,
                Server = _config.Server,
                StartTimestamp = DateTime.Now,
                JobCount = jobQueueCount,
                SegmentTrainInfo = ConvertToSegmentTrainInfo(_segmentConfig),
                SegmentCount = _segmentConfig?.Segments?.Count ?? 0
            };
            
            var batchId = await _cloudPlatform.AddBatchToeRun(batchToeRun);
            Console.WriteLine($"Created batch record with ID: {batchId}");
            return batchId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create batch record: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Loads the SegmentConfig from the configured path
    /// </summary>
    /// <returns>SegmentConfig object, or null if file doesn't exist or can't be loaded</returns>
    private SegmentConfig? LoadSegmentConfig()
    {
        try
        {
            if (string.IsNullOrEmpty(_config.BigToeSegmentPath))
            {
                Console.WriteLine("BigToeSegmentPath is not configured.");
                return null;
            }
            
            if (!File.Exists(_config.BigToeSegmentPath))
            {
                Console.WriteLine($"SegmentConfig file not found at {_config.BigToeSegmentPath}.");
                return null;
            }
            
            string jsonContent = File.ReadAllText(_config.BigToeSegmentPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true // Set this to true to allow trailing commas
                
            };
            var segmentConfig = JsonSerializer.Deserialize<SegmentConfig>(jsonContent, options);
            Console.WriteLine($"Loaded SegmentConfig with {segmentConfig?.Segments?.Count ?? 0} segments.");
            return segmentConfig;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading SegmentConfig: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Converts SegmentConfig to a list of SegmentTrainInfo for BatchToeRun
    /// </summary>
    /// <param name="segmentConfig">The SegmentConfig to convert</param>
    /// <returns>List of SegmentTrainInfo containing segment IDs and their TrainOn status</returns>
    private List<SegmentTrainInfo> ConvertToSegmentTrainInfo(SegmentConfig? segmentConfig)
    {
        if (segmentConfig?.Segments == null || segmentConfig.Segments.Count == 0)
        {
            return new List<SegmentTrainInfo>();
        }
        
        return segmentConfig.Segments
            .Select(s => new SegmentTrainInfo
            {
                SegmentId = s.Id,
                TrainOn = s.TrainOn
            })
            .ToList();
    }
    
    /// <summary>
    /// Uploads the top strategies based on MaxUploadStrategy configuration
    /// </summary>
    /// <param name="batchId">The batch ID to upload strategies to</param>
    private async Task UploadTopStrategiesAsync(string? batchId)
    {
        if (_cloudPlatform == null || string.IsNullOrEmpty(batchId))
        {
            Console.WriteLine("No cloud platform or batch ID available, skipping strategy upload.");
            return;
        }
        
        // Get top strategies from the analysis service
        var topStrategies = await _strategyAnalysisService.GetTopStrategiesAsync();
        
        if (topStrategies.Count == 0)
        {
            Console.WriteLine("No strategies to upload.");
            return;
        }
        
        Console.WriteLine($"Total top strategies from analysis service: {topStrategies.Count}");
        
        // Take only the top MaxUploadStrategy strategies
        var strategiesToUpload = topStrategies.Take(_config.MaxUploadStrategy).ToList();
        
        Console.WriteLine($"Uploading top {strategiesToUpload.Count} strategies (max: {_config.MaxUploadStrategy})");
        
        // Upload each strategy with its validation results
        foreach (var analyzedStrategy in strategiesToUpload)
        {
            try
            {
                // Add validation results to the strategy result
                analyzedStrategy.StrategyResult.StrategyResult.ValidationWithFee001 = analyzedStrategy.Validation001;
                analyzedStrategy.StrategyResult.StrategyResult.ValidationWithFee08 = analyzedStrategy.Validation08;
                analyzedStrategy.StrategyResult.StrategyResult.ValidationWithFee15 = analyzedStrategy.Validation15;
                
                // Upload the strategy result
                string strategyResultId = await _cloudPlatform.AddStrategyResults(batchId, analyzedStrategy.StrategyResult.StrategyResult);
                
                // Upload the segment stats if any exist
                if (analyzedStrategy.StrategyResult.SegmentStats.Any())
                {
                    await _cloudPlatform.AddSegmentStats(batchId, strategyResultId, analyzedStrategy.StrategyResult.SegmentStats);
                }
                
                _totalUploadedStrategies++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload strategy: {ex.Message}");
            }
        }
        
        Console.WriteLine($"Successfully uploaded {_totalUploadedStrategies} strategies to Firebase.");
    }

    private async Task UpdateBatchToeRunAsync(string? batchId, long totalStrategies, long totalUploadedStrategies)
    {
        try
        {
            if (_cloudPlatform != null && !string.IsNullOrEmpty(batchId))
            {
                await _cloudPlatform.UpdateBatchToeRun(batchId, totalStrategies, totalUploadedStrategies);
                Console.WriteLine($"Updated batch record {batchId} with total strategies: {totalStrategies}, uploaded strategies: {totalUploadedStrategies}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update batch record: {ex.Message}");
        }
    }
    
    private void PrintJobList(List<ToeJob> jobs)
    {
        Console.WriteLine("=== Job List ===");
        int idx = 1;
        foreach (var job in jobs)
        {
            Console.WriteLine($"Job #{idx++}: Name: {job.Name}, BigToe Config: {_config.BigToeEnvironmentConfigPath}, TinyToe Config: {job.TinyToeConfigPath}");
        }
        Console.WriteLine("=================");
    }
}
