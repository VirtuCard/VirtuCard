using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class War : Game
{
    //initialize 4 decks here

    
    // Start is called before the first frame update
    public War()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.War));
    }

    /// <summary>
    /// This method initializes the game
    /// </summary>
    public override void InitializeGame()
    {
        CardDeck OGDeck = CreateStandard52Deck();
        CardDeck poneUnplayed = new CardDeck();
        for (int i = 0; i < 26; i++)
        {
            poneUnplayed.AddCard(OGDeck.PopCard());
        }
        GetDeck(DeckChoices.PONEUNPLAYED).AddCards(poneUnplayed);
        GetDeck(DeckChoices.PTWOUNPLAYED).AddCards(OGDeck);
    }

    public override bool VerifyMove(Card cardToPlay)
    {
        return true;
    }

    public override bool VerifyCanSkip(int playerIndex)
    {
        return false;
    }

     /// <summary>
    /// This method is called when a player plays a card
    /// </summary>
    /// <param name="cardToPlay">Card that the player is playing</param>
    /// <param name="playerIndex">index of the player</param>
    /// <returns></returns>
    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        //TODO

        return true;
    }
}
