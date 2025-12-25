namespace Clairvoyance.Domain.Effects;

public class DrawCardEffect : EffectBase, IEffect
{
    public int CardToDraw { get; set; }

    public DrawCardEffect(int cardToDraw)
    {
        CardToDraw = cardToDraw;
    }

    public override void Resolves()
    {
        Ability.Owner.Draw(CardToDraw);
    }

    public override IEffect Clone()
    {
        return new DrawCardEffect(CardToDraw);
    }
}