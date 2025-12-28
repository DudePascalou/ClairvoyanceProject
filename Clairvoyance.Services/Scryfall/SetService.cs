using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Clairvoyance.Services.Scryfall;

/// <summary>
/// Services providing features related to MTG expansions.
/// </summary>
public class SetService
{
    private readonly ScryfallConfiguration _Configuration;
    private readonly IMemoryCache _Cache;

    public SetService(IOptions<ScryfallConfiguration> options, IMemoryCache cache)
    {
        _Configuration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _Cache = cache;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_Configuration.JsonFilePath))
        {
            throw new InvalidOperationException("JSON file path is not configured.");
        }

        string jsonData;
        if (File.Exists(_Configuration.JsonFilePath!))
        {
            jsonData = await File.ReadAllTextAsync(_Configuration.JsonFilePath!, cancellationToken);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(_Configuration.JsonUrl))
            {
                throw new InvalidOperationException("JSON URL is not configured.");
            }
            // Example: Fetch and process data from the configured JSON URL
            using var httpClient = new HttpClient();

            // Build a User-Agent that accurately describes this application (assembly name/version).
            var appName = "Clairvoyance";
            var appVersion = "0.1";
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{appName}/{appVersion}");

            // Set an Accept header (application/json is acceptable).
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync(_Configuration.JsonUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            jsonData = await response.Content.ReadAsStringAsync(cancellationToken);
            // Write content to file for local caching:
            await File.WriteAllTextAsync(_Configuration.JsonFilePath!, jsonData, cancellationToken);
        }

        var sets = JsonSerializer.Deserialize<JsonContent<List<Set>>>(jsonData);
        // Cache the sets for future use
        _Cache.Set("ScryfallSets", sets!.Data, TimeSpan.FromDays(7));
    }

    public Set? GetSetByCode(string code)
    {
        if (_Cache.TryGetValue("ScryfallSets", out List<Set>? sets))
        {
            return sets?.FirstOrDefault(s => string.Equals(s.Code, code, StringComparison.OrdinalIgnoreCase));
        }
        return null;
    }
}
