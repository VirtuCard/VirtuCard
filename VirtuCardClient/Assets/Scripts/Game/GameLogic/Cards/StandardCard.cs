using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardCard : Card
{
    private StandardCardRank rank;
    private StandardCardSuit suit;

    public StandardCard(StandardCardRank rank, StandardCardSuit suit)
    {
        this.rank = rank;
        this.suit = suit;
    }

    /// <summary>
    /// Sets the Suit of the card
    /// </summary>
    /// <param name="suit"></param>
    public void SetSuit(StandardCardSuit suit)
    {
        this.suit = suit;
    }

    /// <summary>
    /// Sets the Rank of the card
    /// </summary>
    /// <param name="rank"></param>
    public void SetRank(StandardCardRank rank)
    {
        this.rank = rank;
    }

    /// <summary>
    /// Returns the suit of the card
    /// </summary>
    /// <returns></returns>
    public StandardCardSuit GetSuit()
    {
        return this.suit;
    }

    /// <summary>
    /// Returns the rank of the card
    /// </summary>
    public StandardCardRank GetRank()
    {
        return this.rank;
    }

    /// <summary>
    /// Compares the card to another. Returns a boolean whether they are equal or not
    /// </summary>
    /// <returns></returns>
    public override bool Compare(Card card)
    {
        if (GetRank() == ((StandardCard)card).GetRank() &&
            GetSuit() == ((StandardCard)card).GetSuit())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// This prints the card in a nice format e.g.
    /// "STANDARD CARD (TWO of HEARTS)"
    /// </summary>
    public override void Print()
    {
        Debug.Log(ToString());
    }

    /// <summary>
    /// Returns a nice string in the format
    /// "Rank of Suit"
    /// with rank and suit capitalized
    /// </summary>
    /// <returns></returns>
    public override string ToNiceString()
    {
        string rankCaps = Enum.GetName(typeof(StandardCardRank), GetRank());
        string suitCaps = Enum.GetName(typeof(StandardCardSuit), GetSuit());
        string rank = rankCaps.Substring(0, 1).ToUpper() + rankCaps.Substring(1).ToLower();
        string suit = suitCaps.Substring(0, 1).ToUpper() + suitCaps.Substring(1).ToLower();
        return rank + " of " + suit;
    }

    /// <summary>
    /// This returns the card in a string format
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "STANDARD CARD (" + Enum.GetName(typeof(StandardCardRank), GetRank()) + " of " + Enum.GetName(typeof(StandardCardSuit), GetSuit()) + ")";
    }
}
