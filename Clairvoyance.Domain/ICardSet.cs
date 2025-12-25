using System.Collections.ObjectModel;

namespace Clairvoyance.Domain;

interface ICardSet
{
    ObservableCollection<Card> Cards { get; set; }
    bool HasLand { get; }

    Card Get(string name);
    IEnumerable<Card> Artifacts();
    IEnumerable<Card> Creatures();
    IEnumerable<Card> Enchantments();
    IEnumerable<Card> Instants();
    IEnumerable<Card> Lands();
    IEnumerable<Card> Planeswalkers();
    IEnumerable<Card> Sorceries();
}
