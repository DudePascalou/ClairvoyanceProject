using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clairvoyance.Domain.Effects;

namespace Clairvoyance.Domain.Abilities
{
    /// <summary>
    /// Interface that defines a triggered ability (603).
    /// </summary>
    public interface ITriggeredAbility : IAbility
    {
        IEffect Effect { get; set; }

        TEffect GetEffect<TEffect>() where TEffect : class, IEffect;
    }
}
