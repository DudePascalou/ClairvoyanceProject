using Clairvoyance.Collections.CardHunter;
using Clairvoyance.Collections.Domain;
using Clairvoyance.Services.Gatherer;
using Clairvoyance.Services.Scryfall;
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
                services.AddMemoryCache();
                services.AddHttpClient();

                services.Configure<AppConfiguration>(hostContext.Configuration.GetSection(nameof(AppConfiguration)));
                services.Configure<CardHunterConfiguration>(hostContext.Configuration.GetSection(nameof(CardHunterConfiguration)));
                services.Configure<ScryfallConfiguration>(hostContext.Configuration.GetSection(nameof(ScryfallConfiguration)));
                services.Configure<GathererConfiguration>(hostContext.Configuration.GetSection(nameof(GathererConfiguration)));

                // Create and register shared JsonSerializerOptions with converters globally
                var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    WriteIndented = true
                };
                jsonOptions.Converters.Add(new LanguageJsonConverter());
                jsonOptions.Converters.Add(new GradingJsonConverter());
                jsonOptions.Converters.Add(new CardIdJsonConverter());
                services.AddSingleton(jsonOptions);

                services.AddTransient<CardHunterLocalRepository>();
                services.AddTransient<CardHunterConnector>();

                services.AddSingleton<SetService>();
                services.AddSingleton<GathererSetService>();
            })
            .Build();

        //var setService = host.Services.GetRequiredService<SetService>();
        //await setService.StartAsync();
        var setService = host.Services.GetRequiredService<GathererSetService>();
        await setService.StartAsync();
        //var connector = host.Services.GetRequiredService<CardHunterConnector>();
        //await connector.DownloadCollection();
    }
}
