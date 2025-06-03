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
            
            // Initialize Firebase if enabled
            ICloudPlatform? cloudPlatform = null;
            if (config.Firebase != null && config.Firebase.Enabled)
            {
                try
                {
                    Console.WriteLine("Initializing Firebase connection...");
                    var firebaseFirestore = new FirebaseFirestore();
                    await firebaseFirestore.Initialize(config.Firebase.ProjectId, config.Firebase.ApiKey);
                    cloudPlatform = firebaseFirestore;
                    Console.WriteLine("Firebase connection established successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to initialize Firebase: {ex.Message}");
                }
            }

            // 3. Create an instance of ToeRunFactory
            IToeRunFactory toeRunFactory = new ToeRunFactory(config);

            // 4. Create a list of ToeJob via the ToeJobFactory
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
}