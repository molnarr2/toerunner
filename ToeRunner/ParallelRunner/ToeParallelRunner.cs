using System.Collections.Concurrent;
using ToeRunner.Model;
using ToeRunner.ToeRun;

namespace ToeRunner.ParallelRunner;

/// <summary>
/// Handles parallel execution of ToeJobs using multiple threads.
/// </summary>
public class ToeParallelRunner
{
    private readonly ToeRunnerConfig _config;
    private readonly IToeRunFactory _toeRunFactory;
    
    /// <summary>
    /// Initializes a new instance of the ToeParallelRunner class.
    /// </summary>
    /// <param name="config">The configuration for the ToeRunner.</param>
    /// <param name="toeRunFactory">The factory for creating IToeRun instances.</param>
    public ToeParallelRunner(ToeRunnerConfig config, IToeRunFactory toeRunFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _toeRunFactory = toeRunFactory ?? throw new ArgumentNullException(nameof(toeRunFactory));
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
            // Create an IToeRun instance using the factory
            IToeRun toeRun = _toeRunFactory.Create(job, threadId);
            
            // Run the IToeRun instance
            await toeRun.RunAsync();
            
            Console.WriteLine($"Thread {threadId} completed job: {job.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Thread {threadId} failed to process job {job.Name}: {ex.Message}");
        }
    }
}
