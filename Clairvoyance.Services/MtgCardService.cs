using Clairvoyance.Domain;
using System.Text.Json;

namespace Clairvoyance.Services
{
    public class MtgCardService
    {
        private readonly ICollection<Set> _sets;
        private readonly IDictionary<string, Card> _cardsByName;

        public MtgCardService()
        {
            _cardsByName = new Dictionary<string, Card>(20000);
            _sets = new List<Set>(200);
        }

        public void LoadCardsDatabase()
        {
            //var filePath = @"C:\Users\PC\Documents\GitHub\mtgtools\mtgtools\Data\LightSets-x.json"; // TODO : find another way to load cards...
            //var jsonFileContent = File.ReadAllText(filePath);
            //dynamic dynSets = JsonSerializer.Deserialize(jsonFileContent);
            //IDictionary<string, JToken> jsonSets = dynSets;

            //foreach (var jsonSet in jsonSets)
            //{
            //    var set = JsonConvert.DeserializeObject<Set>(jsonSet.Value.ToString());
            //    if (set != null)
            //    {
            //        _sets.Add(set);
            //        foreach (var card in set.Cards)
            //        {
            //            if (!_cardsByName.ContainsKey(card.Name))
            //            {
            //                _cardsByName.Add(card.Name, card);
            //            }
            //        }
            //    }
            //}
        }

        public Card? FindByName(string name)
        {
            if (_cardsByName.TryGetValue(name, out Card? value))
            {
                return value;
            }
            else if (name.Contains("TODO AftermathSeparator"))
            {
                // TODO : manage aftermath cards...
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parse a deck list into a <see cref="Deck"/>.
        /// Expected deck list is a set of lines, each composed of the count and the card name.
        /// Lines starting with "//" are ignored.
        /// Lines starting with "SB" are added to sideboard.
        /// </summary>
        /// <param name="deckList">Card names and count list.</param>
        /// <returns>The <see cref="Deck"/>.</returns>
        /// <example>
        /// // Planeswalkers
        /// 1 Nissa, Steward of Elements
        /// // Creatures
        /// 1 Elvish Mystic
        /// </example>
        public Deck ParseDeckList(string name, Format format, string deckList)
        {
            var nbAndNames = deckList.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cards = new List<Card>();
            var sideboardCards = new List<Card>();

            foreach (var nbAndName in nbAndNames)
            {
                if (nbAndName.StartsWith(Resources.CommentPrefix)) continue;

                var toSideboard = nbAndName.StartsWith(Resources.SideboardPrefix);
                var firstSpaceIndex = nbAndName.IndexOf(' ');
                var secondSpaceIndex = nbAndName.IndexOf(' ', firstSpaceIndex + 1);

                if (firstSpaceIndex == -1 && secondSpaceIndex == -1) { continue; }

                // Parse card count (main or sideboard) :
                var cardCount = toSideboard
                    ? int.Parse(nbAndName.Substring(firstSpaceIndex + 1, secondSpaceIndex - firstSpaceIndex))
                    : int.Parse(nbAndName.Substring(0, firstSpaceIndex));
                // Parse card name (main or sideboard) :
                var cardName = toSideboard
                    ? nbAndName.Substring(secondSpaceIndex + 1, nbAndName.Length - secondSpaceIndex - 1)
                    : nbAndName.Substring(firstSpaceIndex + 1, nbAndName.Length - firstSpaceIndex - 1);

                var card = FindByName(cardName);
                if (card != null)
                {
                    //card.Add(card, cardCount, toSideboard);
                    for (int i = 0; i < cardCount; i++)
                    {
                        if (!toSideboard)
                        {
                            cards.Add(card);
                        }
                        else
                        {
                            sideboardCards.Add(card);
                        }
                    }
                }
            }

            return new Deck(name, format, cards, sideboardCards);
        }

        public Card? ParseCardJson(string cardJson)
        {
            return JsonSerializer.Deserialize<Card>(cardJson);
        }

        public Deck ParseDeckListJson(string name, Format format, string deckListJson)
        {
            var cards = JsonSerializer.Deserialize<List<Card>>(deckListJson)!;
            var deck = new Deck(name, format, cards);
            return deck;
        }
    }
}