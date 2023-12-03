using Chiral.Console.Utilities;
using Chiral.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;

namespace Chiral.Console;

internal static class Program
{
    public static async Task Main()
    {
        ConsoleUtility.ClearEntireScreen();

        System.Console.WriteLine(Constants.Banner);
        System.Console.WriteLine(Constants.Version);

        System.Console.WriteLine("Press Ctrl+C to shut down");

        try
        {
            await RunAsync();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Shutdown due to unexpected error: {0}", ex.Message);
        }
        finally
        {
            System.Console.WriteLine("Client is shutting down...");

            await Task.Delay(250);
        }
    }

    #region Private Methods

    private static async Task RunAsync()
    {
        var token = ProcessUtility.CreateCancellationToken();

        var host = new HostBuilder()
            .ConfigureServices(ConfigureServices)
            .Build();

        await host.RunAsync(token);
    }

    #endregion

    #region Private Methods: Configure

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // DI from "Serilog".
        services.AddLogging(AddLogging);

        // Singleton: "IConfiguration".
        services.AddSingleton(AddConfiguration);

        // DI from "Chiral.Network".
        services.AddChiralNetwork();
    }

    #endregion

    #region Private Methods: Add

    private static void AddLogging(ILoggingBuilder builder)
    {
        var path = ProcessUtility.GetExecutionPath();
        var file = Path.Join(path, Constants.LogFileName);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Filter.ByExcluding(Matching.FromSource("Microsoft"))
            .Filter.ByExcluding(Matching.FromSource("System"))
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .WriteTo.File(file)
            .CreateLogger();

        builder.AddSerilog();
    }

    private static IConfiguration AddConfiguration(IServiceProvider provider)
    {
        var path = ProcessUtility.GetExecutionPath();

        return new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile(Constants.SettingsFileName, optional: false, reloadOnChange: true)
            .Build();
    }

    #endregion
}
