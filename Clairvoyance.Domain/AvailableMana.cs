using System;

namespace Clairvoyance.Domain
{
    public class AvailableMana
    {
        /// <summary>
        /// Any color of mana (color only, not colorless).
        /// </summary>
        public int AnyColor { get; set; }
        /// <summary>
        /// Any type of mana (color and colorless).
        /// </summary>
        public int AnyType { get; set; }
        /// <summary>
        /// Colorless mana (that has specifically no color).
        /// </summary>
        public int Colorless { get; set; }
        public int White { get; set; }
        public int Blue { get; set; }
        public int Black { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public bool ProducesOnlyOneMana { get { return (AnyType + AnyColor + Colorless + White + Blue + Black + Red + Green) == 1; } }

        public AvailableMana() : this(string.Empty)
        { }

        public AvailableMana(string rawMana)
        {
            Add(rawMana);
        }

        public void Add(string rawMana)
        {
            var rawTypes = rawMana
                .TrimStart('{')
                .TrimEnd('}')
                .Split(new[] { "}{" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var rawType in rawTypes)
            {
                if (rawType == Mana.AnyType)
                {
                    AnyType++;
                }
                if (rawType == Mana.AnyColor)
                {
                    AnyColor++;
                }
                if (rawType == Mana.Colorless)
                {
                    Colorless++;
                }
                if (rawType == Mana.White)
                {
                    White++;
                }
                if (rawType == Mana.Blue)
                {
                    Blue++;
                }
                if (rawType == Mana.Black)
                {
                    Black++;
                }
                if (rawType == Mana.Red)
                {
                    Red++;
                }
                if (rawType == Mana.Green)
                {
                    Green++;
                }
            }
        }

        public void Add(AvailableMana availableMana)
        {
            if (availableMana != null)
            {
                AnyColor += availableMana.AnyColor;
                AnyType += availableMana.AnyType;
                Colorless += availableMana.Colorless;
                White += availableMana.White;
                Blue += availableMana.Blue;
                Black += availableMana.Black;
                Red += availableMana.Red;
                Green += availableMana.Green;
            }
        }

        public bool IsEnoughFor(TypedMana typedManaCost) // rawMana instead of TypedMana
        {
            var availableMana = Clone();

            // Colorless
            if (typedManaCost.Colorless > 0)
            {
                if (availableMana.Colorless >= typedManaCost.Colorless)
                {
                    availableMana.Colorless -= typedManaCost.Colorless;
                }
                else if (availableMana.AnyType > 0)
                {
                    var remainingColorless = typedManaCost.Colorless - availableMana.Colorless;
                    availableMana.Colorless = 0;
                    availableMana.AnyType -= remainingColorless;
                }
                else
                {
                    return false;
                }
            }
            // White
            if (typedManaCost.White > 0)
            {
                if (availableMana.White >= typedManaCost.White)
                {
                    availableMana.White -= typedManaCost.White;
                }
                else if (availableMana.AnyColor > 0 || availableMana.AnyType > 0)
                {
                    var remainingWhite = typedManaCost.White - availableMana.White;
                    availableMana.White = 0;
                    if (availableMana.AnyColor >= remainingWhite)
                    {
                        availableMana.AnyColor -= remainingWhite;
                    }
                    else if (availableMana.AnyType > 0)
                    {
                        remainingWhite = remainingWhite - availableMana.AnyColor;
                        availableMana.AnyColor = 0;
                        availableMana.AnyType -= remainingWhite;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // Blue
            if (typedManaCost.Blue > 0)
            {
                if (availableMana.Blue >= typedManaCost.Blue)
                {
                    availableMana.Blue -= typedManaCost.Blue;
                }
                else if (availableMana.AnyColor > 0 || availableMana.AnyType > 0)
                {
                    var remainingBlue = typedManaCost.Blue - availableMana.Blue;
                    availableMana.Blue = 0;
                    if (availableMana.AnyColor >= remainingBlue)
                    {
                        availableMana.AnyColor -= remainingBlue;
                    }
                    else if (availableMana.AnyType > 0)
                    {
                        remainingBlue = remainingBlue - availableMana.AnyColor;
                        availableMana.AnyColor = 0;
                        availableMana.AnyType -= remainingBlue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // Black
            if (typedManaCost.Black > 0)
            {
                if (availableMana.Black >= typedManaCost.Black)
                {
                    availableMana.Black -= typedManaCost.Black;
                }
                else if (availableMana.AnyColor > 0 || availableMana.AnyType > 0)
                {
                    var remainingBlack = typedManaCost.Black - availableMana.Black;
                    availableMana.Black = 0;
                    if (availableMana.AnyColor >= remainingBlack)
                    {
                        availableMana.AnyColor -= remainingBlack;
                    }
                    else if (availableMana.AnyType > 0)
                    {
                        remainingBlack = remainingBlack - availableMana.AnyColor;
                        availableMana.AnyColor = 0;
                        availableMana.AnyType -= remainingBlack;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // Red
            if (typedManaCost.Red > 0)
            {
                if (availableMana.Red >= typedManaCost.Red)
                {
                    availableMana.Red -= typedManaCost.Red;
                }
                else if (availableMana.AnyColor > 0 || availableMana.AnyType > 0)
                {
                    var remainingRed = typedManaCost.Red - availableMana.Red;
                    availableMana.Red = 0;
                    if (availableMana.AnyColor >= remainingRed)
                    {
                        availableMana.AnyColor -= remainingRed;
                    }
                    else if (availableMana.AnyType > 0)
                    {
                        remainingRed = remainingRed - availableMana.AnyColor;
                        availableMana.AnyColor = 0;
                        availableMana.AnyType -= remainingRed;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // Green
            if (typedManaCost.Green > 0)
            {
                if (availableMana.Green >= typedManaCost.Green)
                {
                    availableMana.Green -= typedManaCost.Green;
                }
                else if (availableMana.AnyColor > 0 || availableMana.AnyType > 0)
                {
                    var remainingGreen = typedManaCost.Green - availableMana.Green;
                    availableMana.Green = 0;
                    if (availableMana.AnyColor >= remainingGreen)
                    {
                        availableMana.AnyColor -= remainingGreen;
                    }
                    else if (availableMana.AnyType > 0)
                    {
                        remainingGreen = remainingGreen - availableMana.AnyColor;
                        availableMana.AnyColor = 0;
                        availableMana.AnyType -= remainingGreen;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // Generic
            if (typedManaCost.Generic > 0)
            {
                return typedManaCost.Generic <=
                (
                    availableMana.AnyColor +
                    availableMana.AnyType +
                    availableMana.White +
                    availableMana.Blue +
                    availableMana.Black +
                    availableMana.Red +
                    availableMana.Green
                );
            }

            // Not expected to happen :
            if (availableMana.AnyColor < 0 ||
                availableMana.AnyType < 0 ||
                availableMana.White < 0 ||
                availableMana.Blue < 0 ||
                availableMana.Black < 0 ||
                availableMana.Red < 0 ||
                availableMana.Green < 0)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"AT:{AnyType}-AC:{AnyColor}-C:{Colorless}-W:{White}-U:{Blue}-B:{Black}-R:{Red}-G:{Green}";
        }

        public AvailableMana Clone()
        {
            return new AvailableMana()
            {
                AnyColor = AnyColor,
                AnyType = AnyType,
                Colorless = Colorless,
                White = White,
                Blue = Blue,
                Black = Black,
                Red = Red,
                Green = Green,
            };
        }
    }
}