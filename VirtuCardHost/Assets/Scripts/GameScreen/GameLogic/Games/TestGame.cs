using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestGame : Game
{
    private const int MIN_NUM_OF_PLAYERS = 1;
    private const int MAX_NUM_OF_PLAYERS = 10;

    public TestGame()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.TestGame));
    }

    /// <summary>
    /// This method initializes the game
    /// </summary>
    public override void InitializeGame()
    {
        AddCardToDeck(new StandardCard(StandardCardRank.FOUR, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AddCardToDeck(new StandardCard(StandardCardRank.FIVE, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AddCardToDeck(new StandardCard(StandardCardRank.SIX, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AddCardToDeck(new StandardCard(StandardCardRank.SEVEN, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AddCardToDeck(new StandardCard(StandardCardRank.EIGHT, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AddCardToDeck(new StandardCard(StandardCardRank.NINE, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AddCardToDeck(new StandardCard(StandardCardRank.TEN, StandardCardSuit.CLUBS), DeckChoices.UNDEALT);
        AdvanceTurn(true);
    }

    /// <summary>
    /// Returns the maximum limit of players
    /// </summary>
    /// <returns></returns>
    public override int GetMaximumNumOfPlayers()
    {
        return MAX_NUM_OF_PLAYERS;
    }

    /// <summary>
    /// Returns the minimum number of players required to start the game
    /// </summary>
    /// <returns></returns>
    public override int GetMinimumNumOfPlayers()
    {
        return MIN_NUM_OF_PLAYERS;
    }

    /// <summary>
    /// This method is used to verify that the Card that the player wants to play is valid.
    /// It does NOT actually play the card, it only checks if it is possible
    /// </summary>
    /// <param name="cardToPlay"></param>
    /// <returns>True or false depending on validity of move</returns>
    public override bool VerifyMove(Card cardToPlay)
    {
        if (((StandardCard)cardToPlay).GetRank() == StandardCardRank.NINE)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// This method is used to verify that the player can skip their turn
    /// It does NOT actually skip their turn, it only checks if it is possible
    /// </summary>
    /// <param name="playerIndex">The index of the player</param>
    /// <returns>True or false depending on validity of skip</returns>
    public override bool VerifyCanSkip(int playerIndex)
    {
        return true;
    }

    /// <summary>
    /// This method is called when a player plays a card
    /// </summary>
    /// <param name="cardToPlay">Card that the player is playing</param>
    /// <param name="playerIndex">index of the player</param>
    /// <returns></returns>
    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        Debug.Log("Player " + playerIndex + " played " + cardToPlay.ToString());
        AddCardToDeck(cardToPlay, DeckChoices.PLAYED);
        AdvanceTurn(true);
        return true;
    }
}
