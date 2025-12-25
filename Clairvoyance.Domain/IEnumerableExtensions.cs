using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain;

public static class IEnumerableExtensions
{
    public static IEnumerable<IAbility> Clone(this IEnumerable<IAbility> abilities)
    {
        foreach (var ability in abilities)
        {
            yield return ability.Clone();
        }
    }
}