using AngleSharp;
using AngleSharp.Dom;
using Clairvoyance.Collections.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace Clairvoyance.Services.Gatherer;

/// <summary>
/// Official Wizards of the Coast Magic: The Gathering © expansions service.
/// </summary>
public class GathererSetService
{
    //https://gatherer.wizards.com/sets?page=2
    private readonly GathererConfiguration _Configuration;
    private readonly JsonSerializerOptions _JsonSerializerOptions;
    private readonly IMemoryCache _Cache;
    private readonly HttpClient _HttpClient;
    private readonly ILogger _Logger;

    public GathererSetService(IOptions<GathererConfiguration> options, JsonSerializerOptions jsonSerializerOptions,
        IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
    {
        _Configuration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _JsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        _Cache = cache;
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _HttpClient = httpClientFactory.CreateClient(typeof(GathererSetService).FullName!);
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger<GathererSetService>();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        List<SetInfo> allSets;
        if (!File.Exists(_Configuration.SetsJsonFilePath!))
        {
            allSets = await LoadSetsAsync(cancellationToken);
            var allSetsJson = JsonSerializer.Serialize(allSets, _JsonSerializerOptions);
            await File.WriteAllTextAsync(_Configuration.SetsJsonFilePath!, allSetsJson, cancellationToken);
        }
        else
        {
            var allSetsJson = await File.ReadAllTextAsync(_Configuration.SetsJsonFilePath!, cancellationToken);
            allSets = JsonSerializer.Deserialize<List<SetInfo>>(allSetsJson)!;
        }
        _Cache.Set("Gatherer.Sets", allSets);
    }

    private async Task<List<SetInfo>> LoadSetsAsync(CancellationToken cancellationToken)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var pageIndex = 0;
        IHtmlCollection<IElement> setRows;
        List<SetInfo> allSets = [];

        do
        {
            pageIndex++;
            var pageUrl = _Configuration.BaseUrl! + _Configuration.SetsPath! + _Configuration.SetsPaginationQuery! + pageIndex;
            var pageUri = new Uri(pageUrl);
            var pageHtml = await _HttpClient.GetStringAsync(pageUrl, cancellationToken);
            var pageDocument = await context.OpenAsync(req => req.Content(pageHtml).Address(pageUri), cancellationToken);
            setRows = pageDocument.QuerySelectorAll("body main section table tbody tr");
            allSets.AddRange(ProcessSetRows(setRows));
            await Task.Delay(256, cancellationToken);
        } while (setRows.Length > 0);

        return allSets;
    }

    private List<SetInfo> ProcessSetRows(IHtmlCollection<IElement> setRows)
    {
        var setInfos = new List<SetInfo>(setRows.Count);
        foreach (var setRow in setRows)
        {
            var cells = setRow.QuerySelectorAll("td");
            // 0: Logo
            // 1: Name
            // 2: Code
            // 3: Card Count
            // 4: Release Date
            // 5: Languages
            var setInfo = new SetInfo
            {
                Code = cells[2].TextContent.Trim(),
                Name = cells[1].TextContent.Trim(),
                CardCount = int.Parse(cells[3].TextContent.Trim()),
                ReleaseDate = DateOnly.ParseExact(cells[4].TextContent.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Url = new Uri(_Configuration.BaseUrl! + cells[1].QuerySelector("a")!.GetAttribute("href")).ToString()
            };
            setInfos.Add(setInfo);
            _Logger.LogInformation
            (
                "{Code};{Name};{CardCount};{ReleaseDate:yyyy-MM-dd}",
                setInfo.Code,
                setInfo.Name,
                setInfo.CardCount,
                setInfo.ReleaseDate
            );
        }
        return setInfos;
    }

    //private static List<Language> ExtractLanguages(IElement element)
    //{
    //    var allLanguages = element.QuerySelectorAll("span").Select(e => e.TextContent);
    //    var unavailableLanguages = element.QuerySelectorAll("span.opacity-50").Select(e => e.TextContent);
    //    var availableLanguages = allLanguages.Except(unavailableLanguages).ToList();
    //    var languages = new List<Language>(availableLanguages.Count);
    //    foreach (var lang in availableLanguages)
    //    {
    //        var language = Language.ParseFromKey(lang);
    //        if (language.HasValue)
    //        {
    //            languages.Add(language.Value);
    //        }
    //    }
    //    return languages;
    //}
}
