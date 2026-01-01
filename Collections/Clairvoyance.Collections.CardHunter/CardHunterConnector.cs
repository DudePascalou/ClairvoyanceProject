using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Clairvoyance.Collections.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Clairvoyance.Collections.CardHunter;

public class CardHunterConnector
{
    private readonly string _Url;
    private readonly ILogger _Logger;
    private readonly HttpClient _HttpClient;
    private readonly CardHunterLocalRepository _Repository;

    public CardHunterConnector(ILoggerFactory loggerFactory,
        IOptions<CardHunterConfiguration> cardHunterConfig,
        CardHunterLocalRepository cardHunterLocalRepository)
    {
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger<CardHunterConnector>();
        _Url = cardHunterConfig.Value?.Url ?? throw new ArgumentNullException(nameof(cardHunterConfig));
        _Repository = cardHunterLocalRepository ?? throw new ArgumentNullException(nameof(cardHunterLocalRepository));

        _HttpClient = new HttpClient(); // TODO: use IHttpClientFactory
        _HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyScraper/1.0)");
        _HttpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html");
    }

    public async Task DownloadCollection()
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);

        Uri firstPageUri = new(_Url);
        var firstPageHtml = await _HttpClient.GetStringAsync(firstPageUri);
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

        using var nextPage = await _HttpClient.SendAsync(request);
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
            var cardId = hrefParts?.Length >= 4 ? FixExpansionCode(hrefParts[4]) : null;
            var isFoil = cells[1].QuerySelector("b.rouge") != null;
            var grading = cells[2].TextContent.Trim();
            var language = cells[3].TextContent.Trim();

            if (cardId != null && appCardId != null && appCollectionId != null)
            {
                var collectionCard = new CollectionCard
                (
                    cardId: new CardId(cardId),
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
            "Usée" => Grading.Fair,
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

    private static string FixExpansionCode(string cardId)
    {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions -> Not conveniently working with KeyValuePair.
        foreach (var kv in _ExpansionCodeFixes)
        {
            if (cardId.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            {
                return cardId.Replace(kv.Key, kv.Value);
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
        return cardId;
    }

    private static readonly Dictionary<string, string> _ExpansionCodeFixes = new()
    {
        // Card Hunter code -> Official code
        { "10ED","10e" }, // Tenth Edition
        { "15ANS","p15a" }, // 15th Anniversary Cards
        { "2X2_","2x2" }, // Double Masters 2022
        { "30A_","30a" }, // 30th Anniversary Edition
        { "3E","3ed" }, // Revised Edition
        { "3EB","3ed" }, // Revised Edition
        { "3EN","fbb" }, // Foreign Black Border
        { "4E","4ed" }, // Fourth Edition
        { "5E","5ed" }, // Fifth Edition
        { "6E","6ed" }, // Classic Sixth Edition
        { "7E","7ed" }, // Seventh Edition
        { "A","lea" }, // Limited Edition Alpha
        { "A10_","asnc" }, // New Capenna Art Series
        { "A11_","aclb" }, // Battle for Baldur's Gate Art Series
        { "A12_","admu" }, // Dominaria United Art Series
        { "A13_","abro" }, // The Brothers' War Art Series
        { "A14_","aone" }, // Phyrexia: All Will Be One Art Series
        { "A15_","amom" }, // March of the Machine Art Series
        { "A16_","altr" }, // Tales of Middle-earth Art Series
        { "A17_","acmm" }, // Commander Masters Art Series
        { "A18_","awoe" }, // Wilds of Eldraine Art Series
        { "A19_","alci" }, // The Lost Caverns of Ixalan Art Series
        { "A20_","amkm" }, // Murders at Karlov Manor Art Series
        { "A21_","aotj" }, // Outlaws of Thunder Junction Art Series
        { "A22_","amh3" }, // Modern Horizons 3 Art Series
        { "A23_","ablb" }, // Bloomburrow Art Series
        { "A25_","a25" }, // Masters 25
        { "ACRA","aacr" }, // Assassin's Creed Art Series
        { "AKHI","mp2" }, // Amonkhet Invocations
        { "AL","all" }, // Alliances
        { "AN","arn" }, // Arabian Nights
        { "AP","apc" }, // Apocalypse
        { "APAC","palp" }, // Asia Pacific Land Program
        { "AQ","atq" }, // Antiquities
        { "ARC_","oarc" }, // Archenemy Schemes
        { "ARCNB_","oe01" }, // Archenemy: Nicol Bolas Schemes
        { "AS7_","amid" }, // Midnight Hunt Art Series
        { "AS8_","avow" }, // Crimson Vow Art Series
        { "AS9_","aneo" }, // Neon Dynasty Art Series
        { "B","leb" }, // Limited Edition Beta
        { "BD","btd" }, // Beatdown Box Set
        { "BNGCD","tbth" }, // Battle the Horde
        { "BRO_","bro" }, // The Brothers' War
        { "C13_","c13" }, // Commander 2013
        { "C14_","c14" }, // Commander 2014
        { "C15_","c15" }, // Commander 2015
        { "C16_","c16" }, // Commander 2016
        { "C17_","c17" }, // Commander 2017
        { "C18_","c18" }, // Commander 2018
        { "C19_","c19" }, // Commander 2019
        { "C20_","c20" }, // Commander 2020
        { "C21_","c21" }, // Commander 2021
        { "CC1_","cc1" }, // Commander Collection: Green
        { "CC2_","cc2" }, // Commander Collection: Black
        { "CE_","ced" }, // Collectors' Edition
        { "CH","chr" }, // Chronicles
        { "CLASH","cp3" }, // Magic Origins Clash Pack
        { "CM2_","cm2" }, // Commander Anthology Volume II
        { "CMA_","cma" }, // Commander Anthology
        { "CN2_","cn2" }, // Conspiracy: Take the Crown
        { "CP","pcmp" }, // Champs and States
        { "DCILM","plgm" }, // DCI Legend Membership
        { "DDADVD","dvd" }, // Duel Decks Anthology: Divine vs. Demonic
        { "DDAEVG","evg" }, // Duel Decks Anthology: Elves vs. Goblins
        { "DDAGVL","gvl" }, // Duel Decks Anthology: Garruk vs. Liliana
        { "DDAJVC","jvc" }, // Duel Decks Anthology: Jace vs. Chandra
        { "DDAS","aclb" }, // Battle for Baldur's Gate Art Series
        { "DFTAS","adft" }, // Aetherdrift Art Series
        { "DK","drk" }, // The Dark
        { "DRCC","drc" }, // Aetherdrift Commander
        { "DSKAS","adsk" }, // Duskmourn: House of Horror Art Series
        { "E01_","e01" }, // Archenemy: Nicol Bolas
        { "E02_","e02" }, // Explorers of Ixalan
        { "EOEAS","aeoe" }, // Edge of Eternities Art Series
        { "EURO","pelp" }, // European Land Program
        { "EX","exo" }, // Exodus
        { "FDNAS","afdn" }, // Foundations Art Series
        { "FE","fem" }, // Fallen Empires
        { "FINAS","afin" }, // Final Fantasy Art Series
        { "FVD","drb" }, // From the Vault: Dragons
        { "FVE","v09" }, // From the Vault: Exiled
        { "FVL","v11" }, // From the Vault: Legends
        { "FVR","v10" }, // From the Vault: Relics
        { "GK1_","gk1" }, // GRN Guild Kit
        { "GK2_","gk2" }, // RNA Guild Kit
        { "GN2_","gn2" }, // Game Night 2019
        { "GN3_","gn3" }, // Game Night: Free-for-All
        { "GPX","pgpx" }, // Grand Prix Promos
        { "GRC","dci" }, // DCI Promos
        { "GS1_","gs1" }, // Global Series Jiang Yanggu & Mu Yanling
        { "GU","ulg" }, // Urza's Legacy
        { "GURU","pgru" }, // Guru
        { "HM","hml" }, // Homelands
        { "HRV","h2r" }, // Modern Horizons 2 Timeshifts
        { "IA","ice" }, // Ice Age
        { "IN","inv" }, // Invasion
        { "INRAS_","ainr" }, // Innistrad Remastered Art Series
        { "J22_","j22" }, // Jumpstart 2022
        { "J25_","j25" }, // Foundations Jumpstart
        { "JOUCD","tdag" }, // Defeat a God
        { "KHA","akhm" }, // Kaldheim Art Series
        { "LE","leg" }, // Legends
        { "M10_","m10" }, // Magic 2010
        { "M11_","m11" }, // Magic 2011
        { "M12_","m12" }, // Magic 2012
        { "M13_","m13" }, // Magic 2013
        { "M14_","m14" }, // Magic 2014
        { "M15_","m15" }, // Magic 2015
        { "M19_","m19" }, // Core Set 2019
        { "M20_","m20" }, // Core Set 2020
        { "M21_","m21" }, // Core Set 2021
        { "MB2_","mb2" }, // Mystery Booster 2
        { "MBP","pmei" }, // Media and Collaboration Promos
        { "MD1_","md1" }, // Modern Event Deck 2014
        { "ME2_","me2" }, // Masters Edition II
        { "ME3_","me3" }, // Masters Edition III
        { "MH1_","mh1" }, // Modern Horizons
        { "MH2_","mh2" }, // Modern Horizons 2
        { "MH2A","amh2" }, // Modern Horizons 2 Art Series
        { "MH3_","mh3" }, // Modern Horizons 3
        { "MHA","amh1" }, // Modern Horizons Art Series
        { "MI","mir" }, // Mirage
        { "MM","mmq" }, // Mercadian Masques
        { "MM2_","mm2" }, // Modern Masters 2015
        { "MM3_","mm3" }, // Modern Masters 2017
        { "NE","nem" }, // Nemesis
        { "OD","ody" }, // Odyssey
        { "P2_","p02" }, // Portal Second Age
        { "PC2_","pc2" }, // Planechase 2012
        { "PCA_","pca" }, // Planechase Anthology
        { "PCH","hop" }, // Planechase
        { "PCH_","ohop" }, // Planechase Planes
        { "PD1_","h09" }, // Premium Deck Series: Slivers
        { "PD3_","pd2" }, // Premium Deck Series: Fire and Lightning
        { "PK","ptk" }, // Portal Three Kingdoms
        { "PLC2_","opc2" }, // Planechase 2012 Planes
        { "PO","por" }, // Portal
        { "PR","pcy" }, // Prophecy
        { "PS","pls" }, // Planeshift
        { "PVC","dde" }, // Duel Decks: Phyrexia vs. the Coalition
        { "Q06_","q06" }, // Pioneer Challenger Decks 2021
        { "RE","ren" }, // Renaissance
        { "RMH2_","h1r" }, // Modern Horizons 1 Timeshifts
        { "RWK","prw2" }, // RNA Ravnica Weekend
        { "SS1_","ss1" }, // Signature Spellbook: Jace
        { "SS2_","ss2" }, // Signature Spellbook: Gideon
        { "SS3_","ss3" }, // Signature Spellbook: Chandra
        { "ST","sth" }, // Stronghold
        { "ST2K","s00" }, // Starter 2000
        { "STAS","astx" }, // Strixhaven Art Series
        { "STM","sta" }, // Strixhaven Mystical Archive
        { "STS","pss3" }, // M19 Standard Showdown
        { "SUS","psus" }, // Junior Super Series
        { "TDMAS","atdm" }, // Tarkir: Dragonstorm Art Series
        { "TE","tmp" }, // Tempest
        { "THGT","p2hg" }, // Two-Headed Giant Tournament
        { "THSCD","tfth" }, // Face the Hydra
        { "THSH","thp2" }, // Born of the Gods Hero's Path
        { "TLI","plst" }, // The List
        { "U","2ed" }, // Unlimited Edition
        { "UD","uds" }, // Urza's Destiny
        { "UG","ugl" }, // Unglued
        { "UQC","pcel" }, // Celebration Cards
        { "UZ","usg" }, // Urza's Saga
        { "V12_","v12" }, // From the Vault: Realms
        { "V13_","v13" }, // From the Vault: Twenty
        { "V14_","v14" }, // From the Vault: Annihilation
        { "V15_","v15" }, // From the Vault: Angels
        { "V16_","v16" }, // From the Vault: Lore
        { "V17_","v17" }, // From the Vault: Transform
        { "VGD","pvan" }, // Vanguard Series
        { "VI","vis" }, // Visions
        { "W17_","w17" }, // Welcome Deck 2017
        { "WCD","w16" }, // Welcome Deck 2016
        { "WCD01_","wc97" }, // World Championship Decks 1997
        { "WCD02_","wc97" }, // World Championship Decks 1997
        { "WCD03_","wc97" }, // World Championship Decks 1997
        { "WCD04_","wc97" }, // World Championship Decks 1997
        { "WCD05_","wc98" }, // World Championship Decks 1998
        { "WCD06_","wc98" }, // World Championship Decks 1998
        { "WCD07_","wc98" }, // World Championship Decks 1998
        { "WCD08_","wc98" }, // World Championship Decks 1998
        { "WCD09_","wc99" }, // World Championship Decks 1999
        { "WCD10_","wc99" }, // World Championship Decks 1999
        { "WCD11_","wc99" }, // World Championship Decks 1999
        { "WCD12_","wc99" }, // World Championship Decks 1999
        { "WCD13_","wc00" }, // World Championship Decks 2000
        { "WCD14_","wc00" }, // World Championship Decks 2000
        { "WCD15_","wc00" }, // World Championship Decks 2000
        { "WCD16_","wc00" }, // World Championship Decks 2000
        { "WCD17_","wc01" }, // World Championship Decks 2001
        { "WCD18_","wc01" }, // World Championship Decks 2001
        { "WCD19_","wc01" }, // World Championship Decks 2001
        { "WCD20_","wc01" }, // World Championship Decks 2001
        { "WCD21_","wc02" }, // World Championship Decks 2002
        { "WCD22_","wc02" }, // World Championship Decks 2002
        { "WCD23_","wc02" }, // World Championship Decks 2002
        { "WCD24_","wc02" }, // World Championship Decks 2002
        { "WCD25_","wc03" }, // World Championship Decks 2003
        { "WCD26_","wc03" }, // World Championship Decks 2003
        { "WCD27_","wc03" }, // World Championship Decks 2003
        { "WCD28_","wc03" }, // World Championship Decks 2003
        { "WCD29_","wc04" }, // World Championship Decks 2004
        { "WCD30_","wc04" }, // World Championship Decks 2004
        { "WCD31_","wc04" }, // World Championship Decks 2004
        { "WCD32_","wc04" }, // World Championship Decks 2004
        { "WL","wth" }, // Weatherlight
        { "WOTC","pwos" }, // Wizards of the Coast Online Store
        { "ZNA","aznr" } // Zendikar Rising Art Series
    };
}
