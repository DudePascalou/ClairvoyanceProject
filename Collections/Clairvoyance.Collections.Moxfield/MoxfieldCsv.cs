using Clairvoyance.Collections.Domain;
using System.Text;

namespace Clairvoyance.Collections.Moxfield;

internal class MoxfieldCsv
{
    internal static string ToCsv(IEnumerable<CollectionCard> collectionCards)
    {
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("\"Count\",\"Tradelist Count\",\"Name\",\"Edition\",\"Condition\",\"Language\",\"Foil\",\"Tags\",\"Last Modified\",\"Collector Number\",\"Alter\",\"Proxy\",\"Purchase Price\"");
        foreach (var card in collectionCards)
        {
            var csvItem = ToCsvItem(card);
            csvBuilder.AppendLine(csvItem.ToCsvLine());
        }
        return csvBuilder.ToString();
    }

    private static CsvItem ToCsvItem(CollectionCard card)
    {
        return new CsvItem
        {
            Count = 1,
            TradelistCount = 1,
            Name = string.Empty,
            Edition = card.CardId.ExpansionCode,
            Condition = card.Grading,
            Language = card.Language,
            Foil = card.IsFoil,
            Tags = string.Empty,
            LastModified = DateTime.UtcNow,
            CollectorNumber = card.CardId.ExpansionNumber,
            Alter = false,
            Proxy = false,
            PurchasePrice = null
        };
    }

    private sealed class CsvItem
    {
        public int Count { get; init; } = 0;
        public int TradelistCount { get; init; } = 0;
        public string Name { get; init; } = string.Empty;
        public string Edition { get; init; } = string.Empty;
        public string Condition { get; init; } = string.Empty;
        public string Language { get; init; } = string.Empty;
        public bool Foil { get; init; } = false;
        public string Tags { get; init; } = string.Empty;
        public DateTime LastModified { get; init; } = DateTime.MinValue;
        public string CollectorNumber { get; init; } = string.Empty;
        public bool Alter { get; init; } = false;
        public bool Proxy { get; init; } = false;
        public decimal? PurchasePrice { get; init; } = null;

        internal string ToCsvLine()
        {
            const string csvTemplate = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\"";
            return string.Format(csvTemplate,
                Count, // 0
                TradelistCount, // 1
                Name, // 2
                Edition, // 3
                Condition, // 4
                Language, // 5
                Foil ? "Foil" : string.Empty, // 6
                Tags, // 7
                LastModified.ToString("yyyy-MM-dd hh:mm:ss.ffffff"), // Used? // 8
                CollectorNumber, // 9
                Alter ? "True" : string.Empty, // 10
                Proxy ? "True" : string.Empty, // 11
                PurchasePrice?.ToString("F2") ?? string.Empty // 12
            );
        }
    }
}
