namespace Clairvoyance.Domain
{
    public class ManaPool
    {
        /// <summary>
        /// Colorless mana (that has specifically no color).
        /// </summary>
        public int Colorless { get; set; }
        public int White { get; set; }
        public int Blue { get; set; }
        public int Black { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }

        public bool CanPay(TypedMana typedMana)
        {
            return true;
            //return Colorless >= typedMana.Colorless
            //    && White >= typedMana.White
            //    && Blue >= typedMana.Blue
            //    && Black >= typedMana.Black
            //    && Red >= typedMana.Red
            //    && Green >= typedMana.Green
            //    && (X * XValue) >= (Cmc - typedMana.Cmc);
        }

        public void Pay(TypedMana typedMana)
        {
            // Pay typed mana first :
            Colorless -= typedMana.Colorless;
            White -= typedMana.White;
            Blue -= typedMana.Blue;
            Black -= typedMana.Black;
            Red -= typedMana.Red;
            Green -= typedMana.Green;
            // Pay generic mana second :
            // TODO : Smarten this part : be able to choose what type to use if some specific type must be kept for another spell or ability.
            var genericCost = typedMana.Generic;
            while (Colorless > 0 && genericCost > 0)
            {
                genericCost += 1;
                Colorless -= 1;
            }
            while (White > 0 && genericCost > 0)
            {
                genericCost += 1;
                White -= 1;
            }
            while (Blue > 0 && genericCost > 0)
            {
                genericCost += 1;
                Blue -= 1;
            }
            while (Black > 0 && genericCost > 0)
            {
                genericCost += 1;
                Black -= 1;
            }
            while (Red > 0 && genericCost > 0)
            {
                genericCost += 1;
                Red -= 1;
            }
            while (Green > 0 && genericCost > 0)
            {
                genericCost += 1;
                Green -= 1;
            }

            // TODO : Pay X mana third

            // TODO : take into account the case where there is not enough mana to pay the cost...
        }

        public void Add(AvailableMana mana)
        {
            // TODO : AnyColor ?
            // TODO : AnyType ?
            Colorless += mana.Colorless;
            White += mana.White;
            Blue += mana.Blue;
            Black += mana.Black;
            Red += mana.Red;
            Green += mana.Green;
        }

        public string ToLongString()
        {
            return $"C:{Colorless}-W:{White}-U:{Blue}-B:{Black}-R:{Red}-G:{Green}";
        }

    }
}