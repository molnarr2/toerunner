using System;
using System.Threading.Tasks;
using ToeRunner.Model;
using ToeRunner.Plugin;

namespace ToeRunner.Firebase;

public class CloudPlatformFactory
{
    private readonly ToeRunnerConfig _config;
    
    public CloudPlatformFactory(ToeRunnerConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }
    
    public async Task<ICloudPlatform?> CreateCloudPlatformAsync()
    {
        // If Firebase is not enabled, return null
        if (_config.Firebase == null )
        {
            Console.WriteLine("Firebase configuration is missing.");
            return null;
        }
        
        ICloudPlatform cloudPlatform;
        
        try
        {
            // Create either a mock or real implementation based on configuration
            if (_config.Firebase.UseMock)
            {
                Console.WriteLine("Initializing Mock Cloud Platform...");
                cloudPlatform = new MockCloudPlatform();
            }
            else
            {
                Console.WriteLine("Initializing Firebase connection...");
                cloudPlatform = new FirebaseFirestore();
                
                // Initialize the cloud platform
                await cloudPlatform.Initialize(_config.Firebase.ProjectId, _config.Firebase.ApiKey);
            }
            
            Console.WriteLine(_config.Firebase.UseMock 
                ? "Mock Cloud Platform initialized successfully."
                : "Firebase connection established successfully.");
                
            return cloudPlatform;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize cloud platform: {ex.Message}");
            return null;
        }
    }
}
