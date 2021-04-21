using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDeck
{
    private List<Card> cards = new List<Card>();


    /// <summary>
    /// This prints all the cards in the CardDeck by calling each one's overloaded print method
    /// </summary>
    public void Print()
    {
        Debug.Log("------Printing Card Deck-----");
        Debug.Log("Contains: " + cards.Count + " Cards.");
        for (int x = 0; x < cards.Count; x++)
        {
            cards[x].Print();
        }

        Debug.Log("\n");
    }

    /// <summary>
    /// Returns true or false depnding on whether or not the card is present in the deck or not
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool IsCardPresent(Card card)
    {
        for (int x = 0; x < cards.Count; x++)
        {
            if (cards[x].Compare(card))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Adds an entire decks cards into this deck
    /// </summary>
    /// <param name="deck"></param>
    public void AddCards(CardDeck deck)
    {
        cards.AddRange(deck.GetAllCards());
    }

    /// <summary>
    /// Adds an entire list of cards into this deck
    /// </summary>
    /// <param name="cards"></param>
    public void AddCards(List<Card> cards)
    {
        AddCards(cards.ToArray());
    }

    /// <summary>
    /// Adds multiple cards to the card deck at once.
    /// It does NOT put them in randomly, it appends them to the end of the deck
    /// </summary>
    /// <param name="cards"></param>
    public void AddCards(Card[] cards)
    {
        for (int x = 0; x < cards.Length; x++)
        {
            this.cards.Add(cards[x]);
        }
    }

    /// <summary>
    /// Adds a single card to the deck.
    /// It does NOT put it in randomly, it appends it to the end of the deck
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    /// <summary>
    /// Gets a random card from the deck and removes it.
    /// That card is then returned
    /// </summary>
    /// <returns></returns>
    public Card PopCard()
    {
        int cardIndex = UnityEngine.Random.Range(0, cards.Count);

        Card returnCard = GetCard(cardIndex);
        RemoveCard(cardIndex);
        return returnCard;
    }

    /// <summary>
    /// Returns the card residing at a particular index in the card deck
    /// </summary>
    /// <param name="index">0-indexed location of the card</param>
    /// <returns></returns>
    public Card GetCard(int index)
    {
     //   Debug.Log("card deck size: " + cards.Count);
        if (index < cards.Count && index >= 0)
        {
            return cards[index];
        }

        throw new System.Exception("Card Deck of size: " + cards.Count + " does not contain card at index: " + index);
    }

    /// <summary>
    /// Checks if the card rank exists in the card deck
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public bool CheckIfCardRankExists(StandardCardRank rank)
    {
        foreach (Card card in cards)
        {
            if (((StandardCard) card).GetRank() == rank)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a list of cards that match the rank specified in the deck
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public List<Card> GetAllCardsOfSpecificRank(StandardCardRank rank)
    {
        List<Card> list = new List<Card>();

        foreach (Card card in cards)
        {
            if (((StandardCard)card).GetRank() == rank)
            {
                list.Add(card);
            }
        }

        return list;
    }

    /// <summary>
    /// Returns true if the deck is a flush.
    /// Do NOT call this method if the card deck does not contain StandardCards.
    /// </summary>
    /// <returns></returns>
    public bool IsAFlush()
    {
        if (cards.Count < 2)
        {
            return true;
        }

        StandardCardSuit prevSuit = ((StandardCard)cards[0]).GetSuit();
        foreach (Card card in cards)
        {
            StandardCardSuit suit = ((StandardCard)card).GetSuit();
            if (suit != prevSuit)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns true if it is a straight.
    /// Do NOT call this method if the card deck does not contain StandardCards.
    /// </summary>
    /// <returns></returns>
    public bool IsAStraight()
    {
        int lowestIndex = GetIndexOfLowestStandardCardRank();
        int rankToBeat = (int)((StandardCard)cards[lowestIndex]).GetRank();
        List<int> uncheckedIndices = new List<int>();
        for (int x = 0; x < cards.Count; x++)
        {
            if (x != lowestIndex)
            {
                uncheckedIndices.Add(x);
            }
        }

        bool didFindNewIndex = true;
        while (didFindNewIndex)
        {
            didFindNewIndex = false;
            foreach (int index in uncheckedIndices)
            {
                int currentRank = (int)((StandardCard)cards[index]).GetRank();
                if (currentRank == rankToBeat + 1)
                {
                    // if it is one higher than the previous lowest
                    uncheckedIndices.Remove(index);
                    didFindNewIndex = true;
                    rankToBeat = currentRank;
                    break;
                }
            }
        }
        if (uncheckedIndices.Count > 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Returns a new deck or StandardCards sorted by rank
    /// </summary>
    /// <returns></returns>
    public CardDeck SortDeckByRank()
    {
        CardDeck sortedDeck = new CardDeck();
        foreach (int value in Enum.GetValues(typeof(StandardCardRank)))
        {
            foreach(Card card in cards)
            {
                if ((int)((StandardCard)card).GetRank() == value)
                {
                    sortedDeck.AddCard(card);
                }
            }
        }
        return sortedDeck;
    }

    /// <summary>
    /// Returns the index in the deck of cards, which has the lowest rank
    /// </summary>
    /// <returns></returns>
    public int GetIndexOfLowestStandardCardRank()
    {
        int index = int.MaxValue;
        for (int x = 0; x < cards.Count; x++)
        {
            if (cards[x].GetType() == typeof(StandardCard))
            {
                int currentRank = (int)((StandardCard)cards[x]).GetRank();
                if (currentRank < index)
                {
                    index = x;
                }
            }
        }
        return index;
    }

    /// <summary>
    /// Returns all the cards within this deck in a list
    /// </summary>
    /// <returns></returns>
    public List<Card> GetAllCards()
    {
        return cards;
    }

    /// <summary>
    /// Returns the number of cards in the deck
    /// </summary>
    /// <returns></returns>
    public int GetCardCount()
    {
        return cards.Count;
    }

    /// <summary>
    /// Removes the specified card from the deck
    /// </summary>
    /// <param name="card"></param>
    public void RemoveCard(Card card)
    {
        for (int x = 0; x < cards.Count; x++)
        {
            if (cards[x].Compare(card) == true)
            {
                cards.RemoveAt(x);
            }
        }
    }

    /// <summary>
    /// Removes a card from the card deck at the specific index
    /// </summary>
    /// <param name="index"></param>
    public void RemoveCard(int index)
    {
        cards.RemoveAt(index);
    }

    /// <summary>
    /// Removes a list of cards from the card deck
    /// </summary>
    /// <param name="cards"></param>
    public void RemoveCards(List<Card> cards)
    {
        for (int x = cards.Count - 1; x >= 0; x--)
        {
            RemoveCard(cards[x]);
        }
    }


    public void RemoveAllCards()
    {
        cards.Clear();
    }

    /// <summary>
    /// This method shuffles all the cards in the card deck randomly
    /// </summary>
    public void Shuffle()
    {
        cards = cards.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    /// <summary>
    /// The method compares two argument decks against each other to determine if they are identical.
    /// </summary>
    /// <param name="deck1"> A deck to compare the latter deck against </param>
    /// <param name="deck2"> A deck that will be compared against the former deck </param>
    /// <returns> True if the two decks are equal. Else false. </returns>
    public static bool IsEqual(CardDeck deck1, CardDeck deck2)
    {
        if (deck1.GetCardCount() != deck2.GetCardCount())
        {
            return false;
        }

        for (int i = 0; i < deck1.GetCardCount(); i++)
        {
            if (deck1.GetCard(i).Compare(deck2.GetCard(i)) == false)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// This method copies a card deck
    /// </summary>
    public void DuplicateDeck(CardDeck toCopy)
    {
        for (int i = 0; i < GetCardCount(); i++)
        {
            (GetCard(i)).CopyCard(toCopy.GetCard(i));
        }
    }
}