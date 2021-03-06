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
    /// This prints the card in a nice format e.g.
    /// "STANDARD CARD (TWO of HEARTS)"
    /// </summary>
    public override void Print()
    {
        Debug.Log("STANDARD CARD (" + Enum.GetName(typeof(StandardCardRank), GetRank()) + " of " + Enum.GetName(typeof(StandardCardSuit), GetSuit()) + ")");
    }
}
