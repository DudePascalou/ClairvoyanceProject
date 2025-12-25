using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Clairvoyance.Collections.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Clairvoyance.Collections.CardHunter;

public class CHConnector
{
    private const string _AppConfigurationKey = "CardHunter";
    private readonly string _Url;
    private readonly ILogger _Logger;
    private readonly HttpClient http;
    private readonly CollectionLocalRepository _Repository;

    public CHConnector(ILogger<CHConnector> logger,
        IOptions<AppConfiguration> appConfig,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _Logger = logger;
        http = new HttpClient(); // TODO: use IHttpClientFactory
        http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyScraper/1.0)");
        http.DefaultRequestHeaders.Accept.ParseAdd("text/html");
        _Url = appConfig.Value?.CollectionApps?
            .FirstOrDefault(c => string.Equals(c.Key, _AppConfigurationKey, StringComparison.OrdinalIgnoreCase))
            ?.Url ?? throw new InvalidOperationException($"App configuration for '{_AppConfigurationKey}' not found.");
        _Repository = new CollectionLocalRepository(Path.Combine(appConfig.Value?.DatabaseDirectory!, _AppConfigurationKey), jsonSerializerOptions);
    }

    public async Task DownloadCollection()
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);

        Uri firstPageUri = new(_Url);
        var firstPageHtml = await http.GetStringAsync(firstPageUri);
        var firstPageDocument = await context.OpenAsync(req => req.Content(firstPageHtml).Address(firstPageUri));
        var collecTotal = "Total: " + firstPageDocument.QuerySelector("#usercollechead h4 #collectotal").TextContent
            + " " + firstPageDocument.QuerySelector("#usercollechead h5").TextContent;
        // Page title:
        var pageTitle = firstPageDocument.QuerySelector("#usercollecform form h5").TextContent
            + " " + collecTotal;
        _Logger.LogInformation(pageTitle);
        await ParsePageAsync(firstPageDocument);

        var nextPageDocument = await AdvanceToNextPageAsync(context, firstPageDocument);
        while (nextPageDocument != null)
        {
            // Page title:
            pageTitle = nextPageDocument.QuerySelector("#usercollecform form h5").TextContent
                + " " + collecTotal + "############################################################";
            _Logger.LogInformation(pageTitle);

            // Extract cards:
            await ParsePageAsync(nextPageDocument);

            // Polite delay:
            await Task.Delay(500);

            nextPageDocument = await AdvanceToNextPageAsync(context, nextPageDocument);
        }

        _Logger.LogInformation("Done.");
    }

    private async Task<IDocument?> AdvanceToNextPageAsync(IBrowsingContext context, IDocument document)
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
        request.Headers.Referrer = new(_Url);

        using var nextPage = await http.SendAsync(request);
        var html = await nextPage.Content.ReadAsStringAsync();

        var nextPageDocument = await context.OpenAsync(req => req.Content(html).Address(_Url));
        return nextPageDocument;
    }

    private async Task ParsePageAsync(IDocument document)
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
            var expansionNumber = hrefParts?.Length >= 4 ? hrefParts[4][(expansionCode?.Length ?? 0)..] : null;
            var isFoil = cells[1].QuerySelector("b.rouge") != null;
            var grading = cells[2].TextContent.Trim();
            var language = cells[3].TextContent.Trim();

            if (appCardId != null && expansionCode != null && expansionNumber != null && appCollectionId != null)
            {
                var cardId = new Card(expansionCode, expansionNumber).Id;
                var collectionCard = new CollectionCard
                (
                    cardId: cardId,
                    appCardId: appCardId,
                    appCollectionId: appCollectionId,
                    isFoil: isFoil,
                    language: MapLanguage(language),
                    grading: MapGrading(grading)
                );
                cards.Add(collectionCard);
                _Logger.LogInformation("Parsed card: {CollectionCard}", collectionCard);
            }
            else
            {
                _Logger.LogWarning("Could not parse card from row with id '{TrId}' and href '{Href}'", trId, href);
            }
        }
        await _Repository.SaveCardsAsync(cards);
    }

    private static Grading? MapGrading(string chGrading)
    {
        return chGrading switch
        {
            "Neuve" => Grading.NearMintMint,
            "Presque neuve" => Grading.NearMint,
            "Jouée" => Grading.Good,
            "Mâchée" => Grading.Poor,
            _ => null,
        };
    }

    private static Language? MapLanguage(string chLanguage)
    {
        return chLanguage switch
        {
            "FRA" => Language.French,
            "ANG" => Language.English,
            "ALL" => Language.German,
            "ESP" => Language.Spanish,
            "ITA" => Language.Italian,
            "JAP" => Language.Japanese,
            "CHI" => Language.TraditionalChinese,
            "CHS" => Language.SimplifiedChinese,
            "COR" => Language.Korean,
            "RUS" => Language.Russian,
            "POR" => Language.Portuguese,
            _ => null,
        };
    }
}
