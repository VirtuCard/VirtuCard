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
        
        if (playerIndex == 0)
        {
            AddCardToDeck(GetDeck(DeckChoices.PONEUNPLAYED).PopCard(), DeckChoices.PONEPLAYED);
            AdvanceTurn(true);
        }
        else
        {            
            AddCardToDeck(GetDeck(DeckChoices.PTWOUNPLAYED).PopCard(), DeckChoices.PTWOPLAYED);
            StandardCard firstCard = (StandardCard) GetDeck(DeckChoices.PONEPLAYED).GetCard(0);
            StandardCard secondCard = (StandardCard) GetDeck(DeckChoices.PTWOPLAYED).GetCard(0);
            if (firstCard.GetRank() == secondCard.GetRank())
            {
                AdvanceTurn(true);
            }
            else if (firstCard.GetRank() > secondCard.GetRank())
            {
            // Player one has won the turn with a higher ranking card

                // This takes all of player 2's played cards and adds it to player
                // 1's unplayed deck
                for (int i = 0; i < GetDeck(DeckChoices.PTWOPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PONEUNPLAYED).AddCard(GetDeck(DeckChoices.PTWOPLAYED).PopCard());
                }
                // This takes the cards from player 1's played cards and adds it to player
                // 1's unplayed deck
                for (int i = 0; i < GetDeck(DeckChoices.PONEPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PONEUNPLAYED).AddCard(GetDeck(DeckChoices.PONEPLAYED).PopCard());
                }
                AdvanceTurn(true);
            }
            else if (firstCard.GetRank() < secondCard.GetRank())
            {
            // Player two has won the turn with a higher ranking card

                // This takes all of the cards from the player ones played deck and adds it
                // to player 2's unplayed deck
                for (int i = 0; i < GetDeck(DeckChoices.PONEPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PTWOUNPLAYED).AddCard(GetDeck(DeckChoices.PONEPLAYED).PopCard());
                }
                // This takes the cards from player 2's played cards and adds it to player
                // 2's unplayed deck
                for (int i = 0; i < GetDeck(DeckChoices.PONEPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PTWOUNPLAYED).AddCard(GetDeck(DeckChoices.PTWOPLAYED).PopCard());
                }
                AdvanceTurn(true);
            } 
        }
        // checks to see if a player has won
        if (GetDeck(DeckChoices.PONEUNPLAYED).GetCardCount() == 52)
        {
            // declare a winner with raising an event
        }
        else if (GetDeck(DeckChoices.PTWOUNPLAYED).GetCardCount() == 52)
        {
            // declare a winner with raising an event
        }
        return true;
    }
}
