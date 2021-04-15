using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameScreen.GameLogic.Cards;

public class Uno : Game
{
    private const int MIN_NUM_OF_PLAYERS = 1; //TODO: Make this two.
    private const int MAX_NUM_OF_PLAYERS = 10;
    private const int NUM_OF_CARDS_PER_PLAYER = 7;
    private bool direction;

    public Uno()
    {
        direction = true;
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.Uno));
    }

    /// <summary>
    /// This method initializes the game
    /// </summary>
    public override void InitializeGame()
    {
        //Create deck

        CardDeck deck = CreateUnoDeck();
        GetDeck(DeckChoices.UNDEALT).AddCards(deck);
        ShuffleDeck(DeckChoices.UNDEALT);

        // Send cards to players
        List<PlayerInfo> players = GetAllPlayers();
        List<CardDeck> playerDecks = new List<CardDeck>();

        for (int x = 0; x < players.Count; x++)
        {
            playerDecks.Add(new CardDeck());
        }

        for (int deckIndex = 0; deckIndex < players.Count; deckIndex++)
        {
            // each deck receives 7 cards
            for (int numOfCards = 0; numOfCards < NUM_OF_CARDS_PER_PLAYER; numOfCards++)
            {
                playerDecks[deckIndex].AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
            }

            playerDecks[deckIndex].Print();
        }

        // send the cards to the players
        for (int x = 0; x < players.Count; x++)
        {
            PhotonScripts.NetworkController.SendCardsToPlayer(players[x].username, playerDecks[x].GetAllCards(), true,
                false);
        }

        SendOutPlayerTurnIndex();

        // Should I have advance turn?
        // AdvanceTurn(true);
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
        UnoCard currentCard = (UnoCard) cardToPlay;
        UnoCard lastCard = (UnoCard) GetLastPlayedCard();

        if (currentCard.value == UnoCardValue.WILD || currentCard.value == UnoCardValue.PLUS_FOUR)
        {
            return true;
        }

        if (lastCard == null)
        {
            return true;
        }

        return currentCard.value == lastCard.value || currentCard.color == lastCard.color;
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
        UnoCard unoCard = (UnoCard) cardToPlay;

        Debug.Log("Player " + playerIndex + " played " + cardToPlay.ToString());
        AddCardToDeck(cardToPlay, DeckChoices.PLAYED);

        if (unoCard.value == UnoCardValue.REVERSE)
        {
            HostData.SetDoShowNotificationWindow(true,
                GetPlayerOfCurrentTurn().username + " has reversed the direction!");
            direction = !direction;
            AdvanceTurn(direction);
        }
        else if (unoCard.value == UnoCardValue.SKIP)
        {
            HostData.SetDoShowNotificationWindow(true,
                GetPlayerOfCurrentTurn().username + " has skipped " + GetNextPlayer(direction).username + "'s turn!");
            SkipTurn(direction, 1);
        }
        else if (unoCard.value == UnoCardValue.PLUS_TWO)
        {
            HostData.SetDoShowNotificationWindow(true,
                GetPlayerOfCurrentTurn().username + " has given " + GetNextPlayer(direction).username + " 2 cards!");

            CardDeck newCards = new CardDeck();
            for (int i = 0; i < 2; i++)
            {
                newCards.AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
            }

            PhotonScripts.NetworkController.SendCardsToPlayer(GetNextPlayer(direction).username, newCards.GetAllCards(),
                true,
                false);
            SkipTurn(direction, 1);
        }
        else if (unoCard.value == UnoCardValue.PLUS_FOUR)
        {
            HostData.SetDoShowNotificationWindow(true,
                GetPlayerOfCurrentTurn().username + " has given " + GetNextPlayer(direction).username + " 4 cards!");

            CardDeck newCards = new CardDeck();
            for (int i = 0; i < 4; i++)
            {
                newCards.AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
            }

            PhotonScripts.NetworkController.SendCardsToPlayer(GetNextPlayer(direction).username, newCards.GetAllCards(),
                true,
                false);
            SkipTurn(direction, 1);
        }
        else
        {
            AdvanceTurn(direction);
        }

        return true;
    }

    /// <summary>
    /// This is the method that is called when a player has their turn forcefully skipped by the timer
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void ForceSkipTurn(int playerIndex)
    {
        Debug.Log(GetPlayer(playerIndex).username + " was forcefully skipped by the timer");
    }
}