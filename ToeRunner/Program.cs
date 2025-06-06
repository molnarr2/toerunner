using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using ToeRunner.Firebase;
using ToeRunner.Model;
using ToeRunner.Model.Firebase;
using ToeRunner.ParallelRunner;
using ToeRunner.Plugin;
using ToeRunner.Setup;
using ToeRunner.ToeRun;

namespace ToeRunner;

public class Program
{
    
    public static async Task Main(string[] args)
    {
        try
        {
            // 1. Accept one parameter from the command line which is a path to the JSON config
            if (args.Length < 1)
            {
                Console.WriteLine("Error: Please provide a path to the configuration file.");
                Console.WriteLine("Usage: ToeRunner <path-to-config-json>");
                return;
            }

            string configPath = args[0];
            Console.WriteLine($"Reading configuration from: {configPath}");

            // Check if the file exists
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"Error: Configuration file not found at {configPath}");
                return;
            }

            // 2. Use Newtonsoft.Json to parse the JSON to ToeRunnerConfig
            string jsonContent = File.ReadAllText(configPath);
            ToeRunnerConfig config = JsonConvert.DeserializeObject<ToeRunnerConfig>(jsonContent) ?? new ToeRunnerConfig();

            if (config == null)
            {
                Console.WriteLine("Error: Failed to parse configuration file.");
                return;
            }

            Console.WriteLine($"Configuration loaded successfully. Parallel runners: {config.ParallelRunners}");
            
            // Initialize cloud platform if enabled
            ICloudPlatform? cloudPlatform = await InitializeCloudPlatformAsync(config);
            if (cloudPlatform == null)
            {
                return; // Error already logged in InitializeCloudPlatformAsync
            }

            // 3. Create an instance of ToeRunFactory
            IToeRunFactory toeRunFactory = new ToeRunFactory(config, cloudPlatform);

            // 4. Create a list of ToeJob via the ToeJobFactory
            if (config.Runs == null)
            {
                Console.WriteLine("Error: No runs configured in the configuration file.");
                return;
            }
            var toeJobs = ToeJobFactory.CreateToeJobs(config.Runs);
            Console.WriteLine($"Created {toeJobs.Count} jobs to process.");

            // 5. Create an instance of ToeParallelRunner with cloud platform if available
            var parallelRunner = new ToeParallelRunner(config, toeRunFactory, cloudPlatform);

            // 6. Run the ToeParallelRunner::ProcessJobsAsync and wait for it to finish
            Console.WriteLine("Starting parallel processing...");
            await parallelRunner.ProcessJobsAsync(toeJobs);

            // 7. Print out a completed message
            Console.WriteLine("All jobs have been completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    /// <summary>
    /// Initializes the cloud platform based on configuration
    /// </summary>
    /// <param name="config">The application configuration</param>
    /// <returns>An initialized ICloudPlatform instance or null if initialization failed</returns>
    private static async Task<ICloudPlatform?> InitializeCloudPlatformAsync(ToeRunnerConfig config)
    {
        try
        {
            var cloudPlatformFactory = new CloudPlatformFactory(config);
            var cloudPlatform = await cloudPlatformFactory.CreateCloudPlatformAsync();
            
            if (cloudPlatform == null)
            {
                Console.WriteLine("Failed to initialize cloud platform. Exiting.");
                return null;
            }
            
            return cloudPlatform;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize cloud platform: {ex.Message}");
            return null;
        }
    }
}