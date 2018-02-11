using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions
{
    public class CanTapCondition : ICondition
    {
        public IAbility Ability { get; set; }
        public ICondition Clone() { return new CanTapCondition(); }
        public bool IsTrue() { return Ability.Card.CanTap(); }
    }
}
