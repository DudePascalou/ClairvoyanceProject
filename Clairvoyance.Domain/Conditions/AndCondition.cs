using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions
{
    public class AndCondition : ICondition
    {
        private IAbility _ability;
        public IAbility Ability
        {
            get { return _ability; }
            set
            {
                _ability = value;
                Condition1.Ability = value;
                Condition2.Ability = value;
            }
        }
        public ICondition Condition1 { get; set; }
        public ICondition Condition2 { get; set; }

        public AndCondition(ICondition condition1, ICondition condition2)
        {
            Condition1 = condition1;
            Condition2 = condition2;
        }

        public ICondition Clone() { return new AndCondition(Condition1.Clone(), Condition2.Clone()); }
        public bool IsTrue() { return Condition1.IsTrue() && Condition2.IsTrue(); }
    }
}
