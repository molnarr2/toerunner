using System.Collections.Concurrent;
using ToeRunner.Model;
using ToeRunner.Model.Firebase;
using ToeRunner.Plugin;
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
        
        // Create tasks for each thread
        var tasks = new List<Task>();
        for (int i = 0; i < _config.ParallelRunners; i++)
        {
            var threadId = i + 1;
            tasks.Add(Task.Run(() => ProcessJobsThread(jobQueue, threadId)));
        }
        
        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
        
        Console.WriteLine("All jobs have been processed successfully!");
    }
    
    /// <summary>
    /// Thread worker method that processes jobs from the queue.
    /// </summary>
    /// <param name="jobQueue">The thread-safe queue of jobs.</param>
    /// <param name="threadId">The ID of the thread.</param>
    private void ProcessJobsThread(ConcurrentQueue<ToeJob> jobQueue, int threadId)
    {
        while (jobQueue.TryDequeue(out var job))
        {
            ProcessJob(job, threadId);
        }
        
        Console.WriteLine($"Thread {threadId} completed - no more jobs to process.");
    }
    
    /// <summary>
    /// Processes a single job.
    /// </summary>
    /// <param name="job">The job to process.</param>
    /// <param name="threadId">The ID of the thread processing the job.</param>
    private async void ProcessJob(ToeJob job, int threadId)
    {
        Console.WriteLine($"Thread {threadId} processing job: {job.Name} (BigToe: {job.BigToeEnvironmentConfigPath}, TinyToe: {job.TinyToeConfigPath})");
        
        try
        {
            // Create a batch record if cloud platform is available
            var batchId = await CreateBatchRecord(job, threadId);
            
            // Create an IToeRun instance using the factory
            IToeRun toeRun = _toeRunFactory.Create(job, threadId, batchId);
            
            // Run the IToeRun instance
            await toeRun.RunAsync();
            
            // Update batch record with completion time
            if (!string.IsNullOrEmpty(batchId))
            {
                await UpdateBatchRecord(batchId);
            }
            
            Console.WriteLine($"Thread {threadId} completed job: {job.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Thread {threadId} failed to process job {job.Name}: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Creates a batch record in the cloud platform.
    /// </summary>
    /// <param name="job">The job being processed.</param>
    /// <param name="threadId">The ID of the thread processing the job.</param>
    /// <returns>The ID of the created batch record, or null if creation failed.</returns>
    private async Task<string?> CreateBatchRecord(ToeJob job, int threadId)
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
                Name = job.Name,
                Description = $"Job run by thread {threadId}",
                Server = Environment.MachineName,
                StartTimestamp = DateTime.UtcNow,
                SegmentDetails = new List<Model.BigToe.PlaybackSegmentDetails>()
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
    /// Updates a batch record in the cloud platform with completion information.
    /// </summary>
    /// <param name="batchId">The ID of the batch record to update.</param>
    private async Task UpdateBatchRecord(string batchId)
    {
        if (_cloudPlatform == null)
        {
            Console.WriteLine("No cloud platform available, skipping batch record update.");
            return;
        }
        
        try
        {
            var batchToeRun = new BatchToeRun
            {
                Id = batchId,
                EndTimestamp = DateTime.UtcNow
            };
            
            await _cloudPlatform.SaveBatchToeRun(batchToeRun);
            Console.WriteLine($"Updated batch record with ID: {batchId}");
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
            Console.WriteLine($"Job #{idx++}: Name: {job.Name}, BigToe Config: {job.BigToeEnvironmentConfigPath}, TinyToe Config: {job.TinyToeConfigPath}");
        }
        Console.WriteLine("================");
    }
}
