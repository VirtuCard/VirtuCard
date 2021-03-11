using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardCard : Card
{
    private StandardCardRank rank;
    private StandardCardSuit suit;

    /// <summary>
    /// Must pass in a non-null rank and suit to avoid an invalid card
    /// </summary>
    public StandardCard(StandardCardRank rank, StandardCardSuit suit)
    {
        this.rank = rank;
        this.suit = suit;
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
    /// This returns the card in a string format
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "STANDARD CARD (" + Enum.GetName(typeof(StandardCardRank), GetRank()) + " of " + Enum.GetName(typeof(StandardCardSuit), GetSuit()) + ")";
    }
}
