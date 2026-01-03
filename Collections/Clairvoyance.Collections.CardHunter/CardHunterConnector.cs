using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Clairvoyance.Collections.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Clairvoyance.Collections.CardHunter;

public class CardHunterConnector
{
    private readonly CardHunterConfiguration _Configuration;
    private readonly ILogger _Logger;
    private readonly HttpClient _HttpClient;
    private readonly CardHunterLocalRepository _Repository;
    private ICollection<SetInfo> _Sets = [];

    public CardHunterConnector(ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory,
        IOptions<CardHunterConfiguration> cardHunterConfig,
        CardHunterLocalRepository cardHunterLocalRepository)
    {
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger<CardHunterConnector>();
        _Configuration = cardHunterConfig.Value ?? throw new ArgumentNullException(nameof(cardHunterConfig));
        _Repository = cardHunterLocalRepository ?? throw new ArgumentNullException(nameof(cardHunterLocalRepository));

        _HttpClient = httpClientFactory.CreateClient(typeof(CardHunterConnector).FullName!);
        _HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Clairvoyance/1.0)");
        _HttpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html");
    }

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (_Repository.SetsExists())
        {
            _Sets = [.. await _Repository.LoadSetsAsync(cancellationToken)];
        }
        else
        {
            _Sets = await DownloadSetsAsync(cancellationToken);
            await _Repository.SaveSetsAsync(_Sets, cancellationToken);
        }
    }

    private async Task<ICollection<SetInfo>> DownloadSetsAsync(CancellationToken cancellationToken)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);

        var setsPageUri = new Uri(_Configuration.BaseUrl! + _Configuration.SetsPath!);
        var setsPageHtml = await _HttpClient.GetStringAsync(setsPageUri, cancellationToken);
        var setsPageDocument = await context.OpenAsync(req => req.Content(setsPageHtml).Address(setsPageUri), cancellationToken);

        var setItems = setsPageDocument.QuerySelectorAll("div#main-content div.dbx-content div p.gris");
        var sets = new List<SetInfo>(setItems.Length);
        foreach (var setItem in setItems)
        {
            var expansionCode = Path.GetFileNameWithoutExtension(setItem.QuerySelector("a img")!.GetAttribute("src")!).Trim('_');
            var expansionName = setItem.QuerySelector("a img")!.GetAttribute("title")!["Logo ".Length..];
            // [18/06/2021] - 162 cartes
            var regex = new Regex(@"\[(?<date>\d{2}/\d{2}/\d{4})\]\s*-\s*(?<count>\d+)\s*cartes", RegexOptions.IgnoreCase);
            var match = regex.Match(setItem.TextContent);
            var releaseDateText = string.Empty;
            var cardCountText = string.Empty;
            if (match.Success)
            {
                releaseDateText = match.Groups["date"].Value;
                cardCountText = match.Groups["count"].Value;
            }
            var setInfo = new SetInfo
            {
                Code = expansionCode!,
                Name = expansionName,
                CardCount = int.Parse(cardCountText),
                ReleaseDate = DateOnly.ParseExact(releaseDateText, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                Url = new Uri(_Configuration.BaseUrl! + setItem.QuerySelector("a")!.GetAttribute("href")).ToString(),
            };
            sets.Add(setInfo);
        }
        return sets;
    }

    public async Task DownloadCollectionAsync(CancellationToken cancellationToken = default)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);

        Uri firstPageUri = new(_Configuration.BaseUrl! + _Configuration.CollectionPath!);
        var firstPageHtml = await _HttpClient.GetStringAsync(firstPageUri, cancellationToken);
        var firstPageDocument = await context.OpenAsync(req => req.Content(firstPageHtml).Address(firstPageUri), cancellationToken);
        var collecTotal = "Total: " + firstPageDocument.QuerySelector("#usercollechead h4 #collectotal").TextContent
            + " " + firstPageDocument.QuerySelector("#usercollechead h5").TextContent;
        // Page title:
        var pageTitle = firstPageDocument.QuerySelector("#usercollecform form h5").TextContent
            + " " + collecTotal;
        _Logger.LogInformation(pageTitle);
        await ParsePageAsync(firstPageDocument, cancellationToken);

        var nextPageDocument = await AdvanceToNextPageAsync(context, firstPageDocument, cancellationToken);
        while (nextPageDocument != null)
        {
            // Page title:
            pageTitle = nextPageDocument.QuerySelector("#usercollecform form h5").TextContent
                + " " + collecTotal + "############################################################";
            _Logger.LogInformation(pageTitle);

            // Extract cards:
            await ParsePageAsync(nextPageDocument, cancellationToken);

            // Polite delay:
            await Task.Delay(500, cancellationToken);

            nextPageDocument = await AdvanceToNextPageAsync(context, nextPageDocument, cancellationToken);
        }

        _Logger.LogInformation("Done.");
    }

    private async Task<IDocument?> AdvanceToNextPageAsync(IBrowsingContext context, IDocument document,
        CancellationToken cancellationToken = default)
    {
        // Find the form that contains the advsearchnext input
        var nextButton = document.QuerySelector("input[name='advsearchnext']");
        if (nextButton == null)
        {
            _Logger.LogInformation("No advsearchnext input found — reached end or site uses JS navigation.");
            return null;
        }

        var form = nextButton.Closest("form") as IHtmlFormElement;
        if (form == null)
        {
            _Logger.LogInformation("Can't locate parent form for advsearchnext; aborting.");
            return null;
        }

        // Collect form fields
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var element in form.QuerySelectorAll("input, select, textarea"))
        {
            if (element is IHtmlInputElement input)
            {
                var name = input.Name;
                if (string.IsNullOrEmpty(name)) continue;

                var type = (input.Type ?? "text").ToLowerInvariant();

                // For submit/button inputs: include only the advsearchnext button (simulate clicking that one)
                if (type == "submit" || type == "button")
                {
                    if (!string.Equals(name, "advsearchnext", StringComparison.OrdinalIgnoreCase))
                    {
                        continue; // skip other submit buttons like advsearchprev
                    }

                    fields[name] = input.Value ?? string.Empty;
                    continue;
                }

                if (type == "checkbox" || type == "radio")
                {
                    if (!input.IsChecked) continue;
                }

                fields[name] = input.Value ?? string.Empty;
            }
            else if (element is IHtmlSelectElement select)
            {
                var name = select.Name;
                if (string.IsNullOrEmpty(name)) continue;
                fields[name] = select.Value ?? string.Empty;
            }
            else if (element is IHtmlTextAreaElement textarea)
            {
                var name = textarea.Name;
                if (string.IsNullOrEmpty(name)) continue;
                fields[name] = textarea.Value ?? string.Empty;
            }
        }

        //// Ensure advsearchnext is present with its value (defensive)
        var submitValue = nextButton.GetAttribute("value") ?? string.Empty;
        fields["advsearchnext"] = submitValue;

        // Resolve action URI and POST the form
        var action = form.Action ?? document.DocumentUri;
        var actionUri = new Uri(new Uri(document.BaseUri), (string?)action);

        var content = new FormUrlEncodedContent(fields);
        var request = new HttpRequestMessage(HttpMethod.Post, actionUri) { Content = content };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        // include referer for politeness/compatibility
        request.Headers.Referrer = new(_Configuration.BaseUrl! + _Configuration.CollectionPath!);

        using var nextPage = await _HttpClient.SendAsync(request, cancellationToken);
        var html = await nextPage.Content.ReadAsStringAsync(cancellationToken);

        var nextPageDocument = await context.OpenAsync(req => req.Content(html).Address(_Configuration.BaseUrl! + _Configuration.CollectionPath!), cancel: cancellationToken);
        return nextPageDocument;
    }

    private async Task ParsePageAsync(IDocument document, CancellationToken cancellationToken = default)
    {
        var collectionItems = document.QuerySelectorAll("table#usercollectab tr.collecdetail");
        var cards = new List<CollectionCard>(collectionItems.Count);
        foreach (var item in collectionItems)
        {
            var trId = item.GetAttribute("id");
            var appCollectionId = trId?["collec".Length..]; // collec88888888 -> 88888888
            var cells = item.QuerySelectorAll("td");
            var href = cells[0].QuerySelector("span a")?.GetAttribute("href");
            // "/magic/cartes/88888888/XXX/XXX888/"
            var hrefParts = href?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var appCardId = hrefParts?.Length >= 2 ? hrefParts[2] : null;
            var expansionCode = hrefParts?.Length >= 3 ? hrefParts[3] : null;
            var expansionNumber = hrefParts?.Length >= 4 && expansionCode != null
                ? hrefParts[4][expansionCode.Length..] : null;
            var isFoil = cells[1].QuerySelector("b.rouge") != null;
            var grading = cells[2].TextContent.Trim();
            var language = cells[3].TextContent.Trim();

            if (expansionCode != null && expansionNumber != null &&
                appCardId != null && appCollectionId != null)
            {
                var collectionCard = new CollectionCard
                (
                    cardId: new CardId(expansionCode, expansionNumber),
                    appCardId: appCardId,
                    appCollectionId: appCollectionId,
                    grading: grading,
                    language: language,
                    isFoil: isFoil
                );
                cards.Add(collectionCard);
                _Logger.LogInformation("Parsed card: {CollectionCard}", collectionCard);
            }
            else
            {
                _Logger.LogWarning("Could not parse card from row with id '{TrId}' and href '{Href}'", trId, href);
            }
        }
        await _Repository.SaveCardsByExpansionAsync(cards, _Sets, cancellationToken);
    }
}
