using System.Diagnostics;
using System.Text.Json;
using ToeRunner.Conversion;
using ToeRunner.FileOps;
using ToeRunner.Filter;
using ToeRunner.Firebase;
using ToeRunner.Model;
using ToeRunner.Model.BigToe;
using ToeRunner.Model.Firebase;
using ToeRunner.Plugin;

namespace ToeRunner.ToeRun
{
    /// <summary>
    /// Implementation of IToeRun interface for running TinyToe and BigToe processes
    /// </summary>
    public class ToeRunImplementation : IToeRun
    {
        private static int _globalInstanceCounter = 0;
        private readonly int _uniqueInstanceId;
        private readonly ToeRunnerConfig _config;
        private readonly ToeJob _job;
        private readonly int _id;
        private readonly string? _batchToeRunId;
        private readonly decimal _uploadStrategyPercentage;
        private readonly FilterPercentageType _filterPercentageType;
        private readonly ICloudPlatform? _cloudPlatform;
        private int _strategyCount = 0;
        private int _uploadedStrategyCount = 0;

        /// <summary>
        /// Constructor for ToeRunImplementation
        /// </summary>
        /// <param name="config">Configuration for the ToeRunner</param>
        /// <param name="job">The ToeJob to run</param>
        /// <param name="id">Unique identifier for this run</param>
        /// <param name="batchToeRunId">The ID of the batch run in Firebase, can be null</param>
        /// <param name="cloudPlatform">Cloud platform for uploading results, can be null</param>
        public ToeRunImplementation(
            ToeRunnerConfig config, 
            ToeJob job, 
            int id, 
            string? batchToeRunId,
            ICloudPlatform? cloudPlatform)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _id = id;
            _batchToeRunId = batchToeRunId;
            _uploadStrategyPercentage = _config.UploadStrategyPercentage;
            _filterPercentageType = _config.FilterProfitPercentage;
            _cloudPlatform = cloudPlatform;
            _uniqueInstanceId = System.Threading.Interlocked.Increment(ref _globalInstanceCounter);
        }

        private void PrintJobInfo()
        {
            Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Job Info:");
            Console.WriteLine($"[ToeRun-{_uniqueInstanceId}]   Name: {_job.Name}");
            Console.WriteLine($"[ToeRun-{_uniqueInstanceId}]   BigToe Config: {_job.BigToeEnvironmentConfigPath}");
            Console.WriteLine($"[ToeRun-{_uniqueInstanceId}]   TinyToe Config: {_job.TinyToeConfigPath}");
        }

        /// <summary>
        /// Runs the TinyToe and BigToe processes asynchronously
        /// </summary>
        public async Task RunAsync()
        {
            try
            {
                int step = 1;
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Starting RunAsync");
                PrintJobInfo();
                // Validate required configuration
                if (string.IsNullOrEmpty(_config.WorkspacePath))
                {
                    throw new InvalidOperationException($"[ToeRun-{_uniqueInstanceId}] WorkspacePath is not configured in ToeRunnerConfig");
                }
                if (string.IsNullOrEmpty(_config.BigToeExecutablePath))
                {
                    throw new InvalidOperationException($"[ToeRun-{_uniqueInstanceId}] BigToeExecutablePath is not configured in ToeRunnerConfig");
                }
                if (string.IsNullOrEmpty(_config.TinyToeExecutablePath))
                {
                    throw new InvalidOperationException($"[ToeRun-{_uniqueInstanceId}] TinyToeExecutablePath is not configured in ToeRunnerConfig");
                }
                if (string.IsNullOrEmpty(_job.BigToeEnvironmentConfigPath))
                {
                    throw new InvalidOperationException($"[ToeRun-{_uniqueInstanceId}] BigToeEnvironmentConfigPath is not configured in ToeJob");
                }
                if (string.IsNullOrEmpty(_job.TinyToeConfigPath))
                {
                    throw new InvalidOperationException($"[ToeRun-{_uniqueInstanceId}] TinyToeConfigPath is not configured in ToeJob");
                }
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Validated configuration");
                
                // 1. Create required directories
                string baseWorkspacePath = Path.Combine(_config.WorkspacePath, _id.ToString());
                string configFolderPath = Path.Combine(baseWorkspacePath, "config");
                string tinyToeOutputPath = Path.Combine(baseWorkspacePath, "outputTinyToe");
                string bigToeOutputPath = Path.Combine(baseWorkspacePath, "outputBigToe");
                CreateDirectoryIfNotExists(configFolderPath);
                CreateDirectoryIfNotExists(tinyToeOutputPath);
                CreateDirectoryIfNotExists(bigToeOutputPath);
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Created required directories");
                
                // 2. Create output file paths
                string tinyToeOutputFilePath = Path.Combine(tinyToeOutputPath, $"{_job.Name}_tinytoe_output.json");
                string bigToeOutputFilePath = Path.Combine(bigToeOutputPath, $"{_job.Name}_bigtoe_output.json");
                
                // 3. Create config file paths
                string bigToeConfigFilePath = Path.Combine(configFolderPath, $"{_job.Name}_bigtoe_config.json");
                string tinyToeConfigFilePath = Path.Combine(configFolderPath, $"{_job.Name}_tinytoe_config.json");
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Prepared file paths bigToeOutputFilePath: {bigToeOutputFilePath}");
                
                // 4. Replace strings in config files and save to new location
                bool bigToeConfigSuccess = FileStringReplacer.ReplaceStringInFile(
                    _job.BigToeEnvironmentConfigPath,
                    "$OUTPUT$",
                    bigToeOutputFilePath,
                    bigToeConfigFilePath);
                if (!bigToeConfigSuccess)
                {
                    throw new Exception($"[ToeRun-{_uniqueInstanceId}] Failed to create BigToe config file for job {_job.Name}");
                }
                bool tinyToeConfigSuccess = FileStringReplacer.ReplaceStringInFile(
                    _job.TinyToeConfigPath,
                    "$OUTPUT$",
                    tinyToeOutputFilePath,
                    tinyToeConfigFilePath);
                if (!tinyToeConfigSuccess)
                {
                    throw new Exception($"[ToeRun-{_uniqueInstanceId}] Failed to create TinyToe config file for job {_job.Name}");
                }
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Created config files");
                
                // 5. Run TinyToe executable
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Running TinyToe executable: {_config.TinyToeExecutablePath} {tinyToeConfigFilePath}");
                await RunProcessAsync(_config.TinyToeExecutablePath, tinyToeConfigFilePath);
                
                // 6. Run BigToe executable with TinyToe output
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Running BigToe executable: {bigToeConfigFilePath} {tinyToeOutputFilePath}");
                await RunProcessAsync(_config.BigToeExecutablePath, $"{bigToeConfigFilePath} {tinyToeOutputFilePath}");
                
                // 7. Load the result JSON file from BigToe
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Step {step++}: Loading BigToe result file");
                StrategyEvaluationResult? strategyEvaluationResult = LoadBigToeResultFile(bigToeOutputFilePath);
                _strategyCount = strategyEvaluationResult?.ExecutorEvaluationResults?.Count ?? 0;
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Loaded BigToe result with {_strategyCount} executor evaluation results.");
                
                // 8. Convert to list of StrategyResult
                if (strategyEvaluationResult == null)
                {
                    Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Strategy evaluation result is null. Cannot convert to strategy results.");
                    return;
                }
                List<StrategyResultWithSegmentStats> strategyResultsWithStats = StrategyResultConverter.ConvertToStrategyResults(strategyEvaluationResult, _job.RunName, _job.Candlestick);
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Converted {strategyResultsWithStats.Count} strategy results with segment stats.");
                
                // 9. Filter out failed strategies directly using the combined records
                List<StrategyResultWithSegmentStats> filteredResultsWithStats = StrategyFilter.FilterFailedStrategies(
                    strategyResultsWithStats, 
                    _uploadStrategyPercentage, 
                    _filterPercentageType);
                
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Filtered to {filteredResultsWithStats.Count} strategy results after applying filter.");
                
                // 10. Upload strategies to Firebase if cloud platform is available
                if (filteredResultsWithStats.Any() && _cloudPlatform != null && !string.IsNullOrEmpty(_batchToeRunId))
                {
                    // Upload each strategy result and its segment stats
                    foreach (var resultWithStats in filteredResultsWithStats)
                    {
                        // Upload the strategy result
                        string strategyResultId = await _cloudPlatform.AddStrategyResults(_batchToeRunId, resultWithStats.StrategyResult);
                        
                        // Upload the segment stats if any exist
                        if (resultWithStats.SegmentStats.Any())
                        {
                            await _cloudPlatform.AddSegmentStats(_batchToeRunId, strategyResultId, resultWithStats.SegmentStats);
                            Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Uploaded {resultWithStats.SegmentStats.Count} segment stats for strategy {strategyResultId}");
                        }
                    }
                    
                    _uploadedStrategyCount = filteredResultsWithStats.Count;
                    Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Uploaded {_uploadedStrategyCount} strategy results to Firebase with batch ID: {_batchToeRunId}");
                }
                else if (filteredResultsWithStats.Any() && (_cloudPlatform == null || string.IsNullOrEmpty(_batchToeRunId)))
                {
                    Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Found {filteredResultsWithStats.Count} strategy results but cloud platform or batch ID is not available. Results not uploaded.");
                }
                else {
                    Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] No successful strategies found after applying filter. No results uploaded.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Error in ToeRunImplementation.RunAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads the BigToe result JSON file and deserializes it to a StrategyEvaluationResult
        /// </summary>
        /// <param name="filePath">Path to the BigToe output JSON file</param>
        /// <returns>Deserialized StrategyEvaluationResult object</returns>
        private StrategyEvaluationResult? LoadBigToeResultFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"[ToeRun-{_uniqueInstanceId}] BigToe result file not found at {filePath}");
                }
                string jsonContent = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<StrategyEvaluationResult>(jsonContent, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Error loading BigToe result file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a directory if it doesn't exist
        /// </summary>
        /// <param name="path">Path to the directory</param>
        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Runs a process asynchronously
        /// </summary>
        /// <param name="executablePath">Path to the executable</param>
        /// <param name="arguments">Command line arguments</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public int GetStrategyCount()
        {
            return _strategyCount;
        }
        
        public int GetUploadedStrategyCount()
        {
            return _uploadedStrategyCount;
        }
        
        private async Task RunProcessAsync(string executablePath, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                // Read output and error streams asynchronously
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();
                // Wait for the process to exit
                await Task.Run(() => process.WaitForExit());
                string output = await outputTask;
                string error = await errorTask;
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"[ToeRun-{_uniqueInstanceId}] Process error output: {error}");
                }
                if (process.ExitCode != 0)
                {
                    throw new Exception($"[ToeRun-{_uniqueInstanceId}] Process exited with code {process.ExitCode}. Error: {error}");
                }
            }
        }
    }
}
