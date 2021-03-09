using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Game
{
    int playerTurnIndex = -1;
    private CardDeck playedCards = new CardDeck();
    private CardDeck undealtCards = new CardDeck();

    /// <summary>
    /// This method is used to verify that the Card that the player wants to play is valid.
    /// It does NOT actually play the card, it only checks if it is possible
    /// </summary>
    /// <param name="cardToPlay"></param>
    /// <returns>True or false depending on validity of move</returns>
    public abstract bool VerifyMove(Card cardToPlay);

    /// <summary>
    /// This method is used to verify that the player can skip their turn
    /// It does NOT actually skip their turn, it only checks if it is possible
    /// </summary>
    /// <param name="playerIndex">The index of the player</param>
    /// <returns>True or false depending on validity of skip</returns>
    public abstract bool VerifyCanSkip(int playerIndex);

    /// <summary>
    /// Adds cards to the specific deck within the game.
    /// Note the cards are NOT randomly inserted, they are appended
    /// </summary>
    /// <param name="cards">cards to add</param>
    /// <param name="whichDeck">which deck to add the cards to</param>
    public void AddCardsToDeck(Card[] cards, DeckChoices whichDeck)
    {
        CardDeck deck = GetDeck(whichDeck);
        deck.AddCards(cards);
    }

    /// <summary>
    /// Draws and removes a random card from the specified deck
    /// </summary>
    /// <param name="whichDeck">Which deck to draw the card from</param>
    /// <returns></returns>
    public Card DrawCardFromDeck(DeckChoices whichDeck)
    {
        CardDeck deck = GetDeck(whichDeck);
        return deck.PopCard();
    }

    /// <summary>
    /// It draws <paramref name="numOfCards"/> random cards from <paramref name="whichDeck"/> deck and subsequently removes them.
    /// </summary>
    /// <param name="numOfCards">Number of cards to draw and return</param>
    /// <param name="whichDeck">Which deck to draw cards from</param>
    /// <returns>A list containing all the drawn cards</returns>
    public List<Card> DrawCardsFromDeck(int numOfCards, DeckChoices whichDeck)
    {
        int deckSize = GetNumOfCardsInDeck(DeckChoices.UNDEALT);
        if (numOfCards < 0 || numOfCards > deckSize)
        {
            throw new Exception("Cannot draw " + numOfCards + " cards from a deck of size: " + deckSize);
        }

        List<Card> cardList = new List<Card>();
        for (int x = 0; x < numOfCards; x++)
        {
            cardList.Add(DrawCardFromDeck(DeckChoices.UNDEALT));
        }
        return cardList;
    }

    /// <summary>
    /// Adds a card to the specific deck within the game.
    /// Note the card is NOT randomly inserted, it is appended
    /// </summary>
    /// <param name="cards">card to add</param>
    /// <param name="whichDeck">which deck to add the card to</param>
    public void AddCardToDeck(Card card, DeckChoices whichDeck)
    {
        CardDeck deck = GetDeck(whichDeck);
        deck.AddCard(card);
    }

    /// <summary>
    /// Prints the specific deck's contents
    /// </summary>
    /// <param name="whichDeck"></param>
    public void PrintDeck(DeckChoices whichDeck)
    {
        GetDeck(whichDeck).Print();
    }

    /// <summary>
    /// This method returns the number of cards in a specified deck
    /// </summary>
    /// <param name="whichDeck">Which deck to get the number of cards for</param>
    /// <returns>The number of cards</returns>
    public int GetNumOfCardsInDeck(DeckChoices whichDeck)
    {
        CardDeck deck = GetDeck(whichDeck);
        return deck.GetCardCount();
    }

    /// <summary>
    /// Gets the game card deck associated with whichDeck
    /// </summary>
    /// <param name="whichDeck"></param>
    /// <returns></returns>
    public CardDeck GetDeck(DeckChoices whichDeck)
    {
        // Is it the undealt deck
        if ((int)whichDeck == 0)
        {
            return undealtCards;
        }
        // Is it the played cards deck
        else if ((int)whichDeck == 1)
        {
            return playedCards;
        }

        throw new Exception("Invalid deck specified: " + (int)whichDeck + ". Game.cs:79");
    }

}
