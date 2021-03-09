using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardCard : Card
{
    public StandardCardRank rank;
    public StandardCardSuit suit;

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
        Debug.Log("STANDARD CARD (" + Enum.GetName(typeof(StandardCardRank), GetRank()) + " of " + Enum.GetName(typeof(StandardCardSuit), GetSuit()) + ")");
    }
}