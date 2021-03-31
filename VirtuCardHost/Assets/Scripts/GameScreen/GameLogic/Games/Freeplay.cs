using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Freeplay : Game
{
    private const int MIN_NUM_OF_PLAYERS = 2;
    private const int MAX_NUM_OF_PLAYERS = 10;

    // Start is called before the first frame update
    public Freeplay()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.Freeplay));
    }

    public override void InitializeGame()
    {
        GetDeck(DeckChoices.UNDEALT).RemoveAllCards();

        // This eventually will be hooked up to work with the settings that the host
        // chooses on the freeplay setting screen
        if (HostData.getClubsAllowed())
        {
            GetDeck(DeckChoices.UNDEALT).AddCards(CreateDeckOfSuit(StandardCardSuit.CLUBS));
        }

        if (HostData.getDiamondsAllowed())
        {
            GetDeck(DeckChoices.UNDEALT).AddCards(CreateDeckOfSuit(StandardCardSuit.DIAMONDS));
        }

        if (HostData.getHeartsAllowed())
        {
            GetDeck(DeckChoices.UNDEALT).AddCards(CreateDeckOfSuit(StandardCardSuit.HEARTS));
        }

        if (HostData.getSpadesAllowed())
        {
            GetDeck(DeckChoices.UNDEALT).AddCards(CreateDeckOfSuit(StandardCardSuit.SPADES));
        }

        AdvanceTurn(true);
    }

    public override bool VerifyMove(Card cardToPlay)
    {
        // This also will need to be updated according to the settings that the host chooses
        return true;
    }

    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        AddCardToDeck(cardToPlay, DeckChoices.PLAYED);
        AdvanceTurn(true);
        return true;
    }

    protected override void ForceSkipTurn(int playerIndex)
    {
        Debug.Log(GetPlayer(playerIndex).username + " was forcefully skipped by the timer");
    }

    public override bool VerifyCanSkip(int playerIndex)
    {
        return HostData.getSkipTurnAllowed();
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
}