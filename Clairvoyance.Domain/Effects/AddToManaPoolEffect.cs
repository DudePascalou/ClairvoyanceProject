namespace Clairvoyance.Domain.Effects;

public class AddToManaPoolEffect : EffectBase, IEffect
{
    public AvailableMana AvailableMana { get; private set; }

    public AddToManaPoolEffect()
    { }
    public AddToManaPoolEffect(string rawMana)
    {
        AvailableMana = new AvailableMana(rawMana);
    }

    public override void Resolves()
    {
        // 605.3b An activated mana ability doesn’t go on the stack, 
        // so it can’t be targeted, countered, or otherwise responded to. 
        // Rather, it resolves immediately after it is activated.
        Ability.Owner.ManaPool.Add(AvailableMana); // TODO : choose color or type, when required...
    }

    public override IEffect Clone()
    {
        return new AddToManaPoolEffect
        {
            AvailableMana = AvailableMana.Clone()
        };
    }
}