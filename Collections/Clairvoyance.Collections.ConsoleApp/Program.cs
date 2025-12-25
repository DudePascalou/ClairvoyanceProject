using Clairvoyance.Collections.CardHunter;
using Clairvoyance.Collections.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Clairvoyance.Collections.ConsoleApp;

internal static class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    options.IncludeScopes = false;
                });
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<AppConfiguration>(hostContext.Configuration.GetSection("AppConfiguration"));

                // Create and register shared JsonSerializerOptions with converters globally
                var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    WriteIndented = true
                };
                jsonOptions.Converters.Add(new LanguageJsonConverter());
                jsonOptions.Converters.Add(new GradingJsonConverter());
                jsonOptions.Converters.Add(new ICardJsonConverter());
                services.AddSingleton(jsonOptions);

                services.AddTransient<CHConnector>();
            })
            .Build();

        var connector = host.Services.GetRequiredService<CHConnector>();
        await connector.DownloadCollection();
    }
}
