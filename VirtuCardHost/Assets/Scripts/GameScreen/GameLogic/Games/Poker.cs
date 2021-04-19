using PhotonScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker : Game
{
    private const int ANTE = 1;
    private const int STARTING_SCORE = 10;

    private const int NUM_OF_CARDS_PER_PLAYER = 5;

    private const int MIN_NUM_OF_PLAYERS = 1;
    private const int MAX_NUM_OF_PLAYERS = 10;

    private int currentPot;
    private int currentBet ;

    public Poker()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.Poker));
    }

    public override void InitializeGame()
    {
        currentBet = 0;
        CardDeck deck = CreateStandard52Deck();
        GetDeck(DeckChoices.UNDEALT).AddCards(deck);

        // Give out initial cards
        List<PlayerInfo> players = GetAllPlayers();
        List<CardDeck> playerDecks = new List<CardDeck>();

        currentPot = players.Count * ANTE;
        for (int x = 0; x < players.Count; x++)
        {
            playerDecks.Add(new CardDeck());
            players[x].score = STARTING_SCORE - ANTE;
        }

        for (int deckIndex = 0; deckIndex < players.Count; deckIndex++)
        {
            // each deck receives 5 cards
            for (int numOfCards = 0; numOfCards < NUM_OF_CARDS_PER_PLAYER; numOfCards++)
            {
                playerDecks[deckIndex].AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
            }
        }
        // send the cards to the players
        for (int x = 0; x < players.Count; x++)
        {
            NetworkController.SendCardsToPlayer(players[x].username, playerDecks[x].GetAllCards(), true, false);
        }

        SendBetInfo();
        SendOutPlayerTurnIndex();
    }

    private void SendBetInfo()
    {
        Dictionary<string, int> keyValues = new Dictionary<string, int>();
        List<PlayerInfo> players = GetAllPlayers();

        foreach (PlayerInfo player in players)
        {
            keyValues.Add(player.username, player.score);
        }
        NetworkController.SendOutPokerBettingInfo(currentPot, currentBet, keyValues);
    }

    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        throw new System.NotImplementedException();
    }

    public override int GetMaximumNumOfPlayers()
    {
        return MAX_NUM_OF_PLAYERS;
    }

    public override int GetMinimumNumOfPlayers()
    {
        return MIN_NUM_OF_PLAYERS;
    }


    public override bool VerifyCanSkip(int playerIndex)
    {
        return false;
    }

    public override bool VerifyMove(Card cardToPlay)
    {
        return true;
    }

    protected override void ForceSkipTurn(int playerIndex)
    {
        // nothing to do here
    }
}
