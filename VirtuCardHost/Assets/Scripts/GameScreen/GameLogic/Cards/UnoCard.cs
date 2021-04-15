using System;

namespace GameScreen.GameLogic.Cards
{
    public class UnoCard : Card
    {
        public UnoCardColor color;

        /* NOTE: If the value is Wild / Plus 4, color doesn't matter */
        public UnoCardValue value;

        public UnoCard(UnoCardColor color, UnoCardValue value)
        {
            this.color = color;
            this.value = value;
        }

        public UnoCardColor Color
        {
            get => color;
            set => color = value;
        }

        public UnoCardValue Value
        {
            get => value;
            set => this.value = value;
        }

        public override void Print()
        {
        }

        public override bool Compare(Card card)
        {
            if (card.GetType() == typeof(UnoCard))
            {
                if (Value == ((UnoCard) card).Value)
                {
                    if (Value != UnoCardValue.WILD || Value != UnoCardValue.PLUS_FOUR)
                        return Color == ((UnoCard) card).Color;
                    return true;
                }
            }

            return false;
        }

        public override void CopyCard(Card toCopy)
        {
            color = ((UnoCard) toCopy).Color;
            value = ((UnoCard) toCopy).Value;
        }

        public override string ToNiceString()
        {
            string colorCaps = Enum.GetName(typeof(UnoCardColor), color);
            string valueCaps = Enum.GetName(typeof(UnoCardValue), value);
            string colorName = colorCaps.Substring(0, 1).ToUpper() + colorCaps.Substring(1).ToLower();
            string valueName = valueCaps.Substring(0, 1).ToUpper() + valueCaps.Substring(1).ToLower();
            if (value == UnoCardValue.WILD || value == UnoCardValue.PLUS_FOUR)
                return valueName;
            return colorName + " of " + valueName;
        }

        public override string ToString()
        {
            return "UNO CARD (" + Enum.GetName(typeof(UnoCardColor), color) + " of " +
                   Enum.GetName(typeof(UnoCardValue), value) + ")";
        }
    }
}