using AngleSharp;
using Clairvoyance.Collections.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Clairvoyance.Collections.Moxfield;

public class MoxfieldConnector
{
    private readonly MoxfieldConfiguration _Configuration;
    private readonly ILogger _Logger;
    //private readonly HttpClient _HttpClient;
    private readonly MoxfieldLocalRepository _Repository;

    public MoxfieldConnector(ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory,
        IOptions<MoxfieldConfiguration> moxfieldConfig,
        MoxfieldLocalRepository moxfieldLocalRepository)
    {
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger<MoxfieldConnector>();
        _Configuration = moxfieldConfig.Value ?? throw new ArgumentNullException(nameof(moxfieldConfig));
        _Repository = moxfieldLocalRepository ?? throw new ArgumentNullException(nameof(moxfieldLocalRepository));

        //_HttpClient = httpClientFactory.CreateClient(typeof(MoxfieldConnector).FullName!);
        //_HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:146.0) Gecko/20100101 Firefox/146.0");
        //_HttpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        //_HttpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
        //_HttpClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
        //_HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        //_HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        //_HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
    }

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (!_Repository.SetsExists())
        {
            var sets = await DownloadSetsAsync(cancellationToken);
            await _Repository.SaveSetsAsync(sets, cancellationToken);
        }
    }

    private async Task<ICollection<SetInfo>> DownloadSetsAsync(CancellationToken cancellationToken)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);

        var setsPageUri = new Uri(_Configuration.BaseUrl! + _Configuration.SetsPath!);
        // Moxfield uses a Cloudflare protection page that forbids programmatic access to the web pages.
        //var setsPageHtml = await _HttpClient.GetStringAsync(setsPageUri, cancellationToken);
        // Load the static copy-pasted HTML content instead:
        var setsPageHtml = File.ReadAllText("Clairvoyance.Collections.Moxfield.Sets.html");
        var setsPageDocument = await context.OpenAsync(req => req.Content(setsPageHtml).Address(setsPageUri), cancellationToken);

        var setItems = setsPageDocument.QuerySelectorAll("main#maincontent div.container table.table tbody tr");
        var sets = new List<SetInfo>(setItems.Length);
        foreach (var setItem in setItems)
        {
            var cells = setItem.QuerySelectorAll("td");
            var expansionName = cells[0].QuerySelector("a div.flex-grow-1")!.TextContent.Trim('\n').Trim();
            var expansionCode = cells[1].TextContent.Trim('\n').Trim();
            var cardCountText = cells[2].TextContent.Trim('\n').Trim();
            var releaseDateText = cells[3].TextContent.Trim('\n').Trim();
            var setInfo = new SetInfo
            {
                Code = expansionCode!,
                Name = expansionName,
                CardCount = int.Parse(cardCountText),
                ReleaseDate = DateOnly.ParseExact(releaseDateText, "MMM d, yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US")),
                Url = cells[0].QuerySelector("a")!.GetAttribute("href")!,
            };
            sets.Add(setInfo);
        }
        return [.. sets.OrderByDescending(s => s.ReleaseDate)];
    }

    public async Task GenerateImportCsvAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogInformation("Generating Moxfield import CSV...");

        var collectionCards = await _Repository.LoadCollectionAsync(cancellationToken);
        // TODO: card name is missing...
        var csvContent = MoxfieldCsv.ToCsv(collectionCards);
        var importFilePath = Path.Combine(_Configuration.BaseDirectory!, $"_{DateTime.Now:yyyyMMdd}_MoxfieldImport.csv");
        await File.WriteAllTextAsync(importFilePath, csvContent, cancellationToken);

        _Logger.LogInformation("Moxfield import CSV saved to: {ImportFilePath}", importFilePath);
    }
}
