using Clairvoyance.Collections.CardHunter;
using Clairvoyance.Collections.Domain;
using Clairvoyance.Collections.Moxfield;
using Microsoft.Extensions.Logging;

namespace Clairvoyance.Collections.ConsoleApp.Mappers;

public class CardHunterToMoxfieldMapper
{
    private readonly ILogger _Logger;
    private readonly CardHunterLocalRepository _CardHunterLocalRepository;
    private readonly MoxfieldLocalRepository _MoxfieldLocalRepository;

    public CardHunterToMoxfieldMapper(
        ILoggerFactory loggerFactory,
        CardHunterLocalRepository cardHunterLocalRepository,
        MoxfieldLocalRepository moxfieldLocalRepository)
    {
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger<CardHunterToMoxfieldMapper>();

        _CardHunterLocalRepository = cardHunterLocalRepository;
        _MoxfieldLocalRepository = moxfieldLocalRepository;
    }


    public async Task MapCollectionsAsync(CancellationToken token = default)
    {
        var cardHunterSets = (await _CardHunterLocalRepository.LoadSetsAsync(token)).ToList();
        var moxfieldSets = (await _MoxfieldLocalRepository.LoadSetsAsync(token)).ToList();

        _Logger.LogInformation("Loading Card Hunter collection...");
        var cardHunterCards = (await _CardHunterLocalRepository.LoadCollectionAsync(token)).ToList();
        var moxfieldCards = new List<CollectionCard>(cardHunterCards.Count);
        foreach (var card in cardHunterCards)
        {
            var mappedCard = new CollectionCard
            (
                appCollectionId: string.Empty,
                appCardId: string.Empty,
                cardId: new CardId(
                    expansionCode: MapExpansionCode(card.CardId.ExpansionCode, cardHunterSets, moxfieldSets),
                    expansionNumber: card.CardId.ExpansionNumber),
                grading: MapGrading(card.Grading),
                language: MapLanguage(card.Language),
                isFoil: card.IsFoil
            );
            moxfieldCards.Add(mappedCard);
        }

        _Logger.LogInformation("Saving Moxfield collection...");
        await _MoxfieldLocalRepository.SaveCollectionAsync(moxfieldCards, token);
    }

    private static string MapExpansionCode(string sourceExpansionCode,
        ICollection<SetInfo> cardHunterSets, ICollection<SetInfo> moxfieldSets)
    {
        //var sourceSet = cardHunterSets
        //    .First(s => s.Code.Equals(sourceExpansionCode, StringComparison.OrdinalIgnoreCase));

        // Expansion code only:
        var targetSet = moxfieldSets
            .FirstOrDefault(s => s.Code.Equals(sourceExpansionCode, StringComparison.OrdinalIgnoreCase));
        if (targetSet != null)
        {
            return targetSet.Code;
        }

        // Fallback: hardcoded mapping
        if (_ExpansionsMapping.TryGetValue(sourceExpansionCode, out var mappedExpansionCode))
        {
            return mappedExpansionCode;
        }

        return SetInfo.Unknown.Code;
    }

    private static readonly Dictionary<string, string> _ExpansionsMapping = new (StringComparer.OrdinalIgnoreCase)
    {
        { "10ED","10e" }, // Tenth Edition
        { "3E","3ed" }, // Revised Edition
        { "4E","4ed" }, // Fourth Edition
        { "5E","5ed" }, // Fifth Edition
        { "6E","6ed" }, // Classic Sixth Edition
        { "7E","7ed" }, // Seventh Edition
        { "A","lea" }, // Limited Edition Alpha
        { "AKHI","mp2" }, // Amonkhet Invocations
        { "AL","all" }, // Alliances
        { "AN","arn" }, // Arabian Nights
        { "AP","apc" }, // Apocalypse
        { "AQ","atq" }, // Antiquities
        { "B","leb" }, // Limited Edition Beta
        { "CH","chr" }, // Chronicles
        { "DDADVD","dvd" }, // Duel Decks Anthology: Divine vs. Demonic
        { "DDAEVG","evg" }, // Duel Decks Anthology: Elves vs. Goblins
        { "DDAGVL","gvl" }, // Duel Decks Anthology: Garruk vs. Liliana
        { "DDAJVC","jvc" }, // Duel Decks Anthology: Jace vs. Chandra
        { "DK","drk" }, // The Dark
        { "DRCC","drc" }, // Aetherdrift Commander
        { "EX","exo" }, // Exodus
        { "FE","fem" }, // Fallen Empires
        { "FVD","drb" }, // From the Vault: Dragons
        { "FVE","v09" }, // From the Vault: Exiled
        { "FVL","v11" }, // From the Vault: Legends
        { "FVR","v10" }, // From the Vault: Relics
        { "GU","ulg" }, // Urza's Legacy
        { "HM","hml" }, // Homelands
        { "HRV","h2r" }, // Modern Horizons 2 Timeshifts
        { "IA","ice" }, // Ice Age
        { "IN","inv" }, // Invasion
        { "LE","leg" }, // Legends
        { "MI","mir" }, // Mirage
        { "MM","mmq" }, // Mercadian Masques
        { "MYB","cmb1" }, // Mystery Booster Playtest Cards 2019
        { "NE","nem" }, // Nemesis
        { "OD","ody" }, // Odyssey
        { "PCH","hop" }, // Planechase
        { "PCH_","ohop" }, // Planechase Planes
        { "PD1_","h09" }, // Premium Deck Series: Slivers
        { "PLC2_","opc2" }, // Planechase 2012 Planes
        { "PR","pcy" }, // Prophecy
        { "PS","pls" }, // Planeshift
        { "PVC","dde" }, // Duel Decks: Phyrexia vs. the Coalition
        { "RE","ren" }, // Renaissance
        { "RMH2_","h1r" }, // Modern Horizons 1 Timeshifts
        { "ST","sth" }, // Stronghold
        { "STM","sta" }, // Strixhaven Mystical Archive
        { "TE","tmp" }, // Tempest
        { "TLI","plst" }, // The List
        { "U","2ed" }, // Unlimited Edition
        { "UD","uds" }, // Urza's Destiny
        { "UG","ugl" }, // Unglued
        { "UZ","usg" }, // Urza's Saga
        { "VI","vis" }, // Visions
        { "WL","wth" } // Weatherlight
    };

    private static string MapGrading(string sourceGrading)
    {
        return sourceGrading switch
        {
            CardHunterGrading.Neuve => MoxfieldGrading.NearMint,
            CardHunterGrading.PresqueNeuve => MoxfieldGrading.SlightlyPlayed,
            CardHunterGrading.Jouee => MoxfieldGrading.ModeratelyPlayed,
            CardHunterGrading.Usee => MoxfieldGrading.HeavilyPlayed,
            CardHunterGrading.Machee => MoxfieldGrading.Damaged,
            _ => string.Empty,
        };
    }

    private static string MapLanguage(string sourceLanguage)
    {
        return sourceLanguage switch
        {
            CardHunterLanguage.Allemand => MoxfieldLanguage.German,
            CardHunterLanguage.Anglais => MoxfieldLanguage.English,
            CardHunterLanguage.Espagnol => MoxfieldLanguage.Spanish,
            CardHunterLanguage.Portugais => MoxfieldLanguage.Portuguese,
            CardHunterLanguage.Italien => MoxfieldLanguage.Italian,
            CardHunterLanguage.Japonais => MoxfieldLanguage.Japanese,
            CardHunterLanguage.Coreen => MoxfieldLanguage.Korean,
            CardHunterLanguage.Russe => MoxfieldLanguage.Russian,
            CardHunterLanguage.ChinoisSimplifie => MoxfieldLanguage.SimplifiedChinese,
            CardHunterLanguage.ChinoisTraditionnel => MoxfieldLanguage.TraditionalChinese,
            _ => string.Empty,
        };
    }
}
