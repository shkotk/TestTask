using Microsoft.Extensions.Configuration;

namespace TestTask.EndToEndTests.Helpers;

public static class ConfigManager
{
    static ConfigManager()
    {
        var configBuilder = new ConfigurationBuilder();

        configBuilder.AddJsonFile("appsettings.EndToEndTests.json", optional: false)
            .AddEnvironmentVariables();

        Configuration = configBuilder.Build();
    }

    public static IConfiguration Configuration { get; }
}