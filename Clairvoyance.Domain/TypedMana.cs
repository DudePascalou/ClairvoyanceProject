namespace Clairvoyance.Domain;

public struct TypedMana
{
    private readonly string _RawMana;
    /// <summary>
    /// Generic mana ("unaware" of any type or color).
    /// </summary>
    public int Generic { get; set; }
    /// <summary>
    /// Number of X mana cost symbol.
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// Value given to X mana cost.
    /// </summary>
    public int XValue { get; set; }
    /// <summary>
    /// Colorless mana (that has specifically no color).
    /// </summary>
    public int Colorless { get; set; }
    public int White { get; set; }
    public int Blue { get; set; }
    public int Black { get; set; }
    public int Red { get; set; }
    public int Green { get; set; }
    /// <summary>
    /// Converted Mana Cost.
    /// </summary>
    public int Cmc { get { return Generic + Colorless + White + Blue + Black + Red + Green; } }

    public TypedMana(string rawMana)
    {
        _RawMana = rawMana;

        Generic = 0;
        X = 0;
        XValue = 0;
        Colorless = 0;
        White = 0;
        Blue = 0;
        Black = 0;
        Red = 0;
        Green = 0;

        var rawTypes = rawMana
            .TrimStart('{')
            .TrimEnd('}')
            .Split(new[] { "}{" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawType in rawTypes)
        {
            if (rawType == Mana.X)
            {
                X++;
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
            else if (int.TryParse(rawType, out int genericMana))
            {
                Generic = genericMana;
            }
        }
    }

    public override string ToString()
    {
        return _RawMana;
    }

    public string ToLongString()
    {
        return $"X:{X}-G:{Generic}-C:{Colorless}-W:{White}-U:{Blue}-B:{Black}-R:{Red}-G:{Green}";
    }

    //public string ToRawString()
    //{
    //    return $"X:{X}-G:{Generic}-C:{Colorless}-W:{White}-U:{Blue}-B:{Black}-R:{Red}-G:{Green}";
    //}

    public static TypedMana Parse(string rawMana)
    {
        return new TypedMana(rawMana);
    }

    public static TypedMana operator +(TypedMana tm1, TypedMana tm2)
    {
        return new TypedMana
        {
            X = tm1.X + tm2.X,
            XValue = tm1.XValue + tm2.XValue,
            Generic = tm1.Generic + tm2.Generic,
            Colorless = tm1.Colorless + tm2.Colorless,
            White = tm1.White + tm2.White,
            Blue = tm1.Blue + tm2.Blue,
            Black = tm1.Black + tm2.Black,
            Red = tm1.Red + tm2.Red,
            Green = tm1.Green + tm2.Green
        };
    }

    public static TypedMana operator -(TypedMana tm1, TypedMana tm2)
    {
        return new TypedMana
        {
            X = tm1.X - tm2.X,
            XValue = tm1.XValue - tm2.XValue,
            Generic = tm1.Generic - tm2.Generic,
            Colorless = tm1.Colorless - tm2.Colorless,
            White = tm1.White - tm2.White,
            Blue = tm1.Blue - tm2.Blue,
            Black = tm1.Black - tm2.Black,
            Red = tm1.Red - tm2.Red,
            Green = tm1.Green - tm2.Green
        };
    }

    public TypedMana Clone()
    {
        return Parse(_RawMana);
    }
}