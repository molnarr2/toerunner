using System.Diagnostics;
using System.Text.Json;
using ToeRunner.FileOps;
using ToeRunner.Model;
using ToeRunner.Model.BigToe;

namespace ToeRunner.ToeRun
{
    /// <summary>
    /// Implementation of IToeRun interface for running TinyToe and BigToe processes
    /// </summary>
    public class ToeRunImplementation : IToeRun
    {
        private readonly ToeRunnerConfig _config;
        private readonly ToeJob _job;
        private readonly int _id;

        /// <summary>
        /// Constructor for ToeRunImplementation
        /// </summary>
        /// <param name="config">Configuration for the ToeRunner</param>
        /// <param name="job">The ToeJob to run</param>
        /// <param name="id">Unique identifier for this run</param>
        public ToeRunImplementation(ToeRunnerConfig config, ToeJob job, int id)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _id = id;
        }

        /// <summary>
        /// Runs the TinyToe and BigToe processes asynchronously
        /// </summary>
        public async Task RunAsync()
        {
            try
            {
                // 1. Create required directories
                string baseWorkspacePath = Path.Combine(_config.WorkspacePath, _id.ToString());
                string configFolderPath = Path.Combine(baseWorkspacePath, "config");
                string tinyToeOutputPath = Path.Combine(baseWorkspacePath, "outputTinyToe");
                string bigToeOutputPath = Path.Combine(baseWorkspacePath, "outputBigToe");

                CreateDirectoryIfNotExists(configFolderPath);
                CreateDirectoryIfNotExists(tinyToeOutputPath);
                CreateDirectoryIfNotExists(bigToeOutputPath);

                // 2. Create output file paths
                string tinyToeOutputFilePath = Path.Combine(tinyToeOutputPath, $"{_job.Name}_tinytoe_output.json");
                string bigToeOutputFilePath = Path.Combine(bigToeOutputPath, $"{_job.Name}_bigtoe_output.json");

                // 3. Create config file paths
                string bigToeConfigFilePath = Path.Combine(configFolderPath, $"{_job.Name}_bigtoe_config.json");
                string tinyToeConfigFilePath = Path.Combine(configFolderPath, $"{_job.Name}_tinytoe_config.json");

                // 4. Replace strings in config files and save to new location
                bool bigToeConfigSuccess = FileStringReplacer.ReplaceStringInFile(
                    _job.BigToeEnvironmentConfigPath,
                    "$OUTPUT$",
                    bigToeOutputFilePath,
                    bigToeConfigFilePath);

                if (!bigToeConfigSuccess)
                {
                    throw new Exception($"Failed to create BigToe config file for job {_job.Name}");
                }

                bool tinyToeConfigSuccess = FileStringReplacer.ReplaceStringInFile(
                    _job.TinyToeConfigPath,
                    "$OUTPUT$",
                    tinyToeOutputFilePath,
                    tinyToeConfigFilePath);

                if (!tinyToeConfigSuccess)
                {
                    throw new Exception($"Failed to create TinyToe config file for job {_job.Name}");
                }

                // 5. Run TinyToe executable
                await RunProcessAsync(_config.TinyToeExecutablePath, tinyToeConfigFilePath);

                // 6. Run BigToe executable with TinyToe output
                await RunProcessAsync(_config.BigToeExecutablePath, $"{bigToeConfigFilePath} {tinyToeOutputFilePath}");

                // 7. Load the result JSON file from BigToe
                StrategyEvaluationResult strategyEvaluationResult = LoadBigToeResultFile(bigToeOutputFilePath);
                Console.WriteLine($"Loaded BigToe result with {strategyEvaluationResult?.ExecutorEvaluationResults?.Count ?? 0} executor evaluation results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToeRunImplementation.RunAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads the BigToe result JSON file and deserializes it to a StrategyEvaluationResult
        /// </summary>
        /// <param name="filePath">Path to the BigToe output JSON file</param>
        /// <returns>Deserialized StrategyEvaluationResult object</returns>
        private StrategyEvaluationResult LoadBigToeResultFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"BigToe result file not found at {filePath}");
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
                Console.WriteLine($"Error loading BigToe result file: {ex.Message}");
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
                    Console.WriteLine($"Process error output: {error}");
                }
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Process exited with code {process.ExitCode}. Error: {error}");
                }
            }
        }
    }
}
