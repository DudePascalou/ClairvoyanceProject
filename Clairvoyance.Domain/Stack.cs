using System.Collections.Generic;

namespace Clairvoyance.Domain
{
    public class Stack
    {
        public Stack<IStackable> SpellsOrEffects { get; set; }

        public Stack()
        {
            SpellsOrEffects = new Stack<IStackable>();
        }

        public void Put(IStackable spellOrEffect)
        {
            SpellsOrEffects.Push(spellOrEffect);
        }

        public void Resolves()
        {
            while (SpellsOrEffects.Peek() != null)
            {
                var spellOfEffect = SpellsOrEffects.Pop();
                spellOfEffect.Resolves();
            }
        }
    }
}