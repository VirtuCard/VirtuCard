using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoFish : Game
{
    // number of cards each player starts with
    private const int NUM_OF_CARDS_PER_PLAYER = 5;

    public GoFish()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.GoFish));
    }

    /// <summary>
    /// This method initializes the game
    /// </summary>
    public override void InitializeGame()
    {
        
        CardDeck deck = CreateStandard52Deck();
        GetDeck(DeckChoices.UNDEALT).AddCards(deck);

        // distribute out cards to players making sure not to give out four of a kind to a player

        List<PlayerInfo> players = GetAllPlayers();

        List<CardDeck> playerDecks = new List<CardDeck>();
        for (int x = 0; x < players.Count; x++)
        {
            playerDecks.Add(new CardDeck());
        }

        for (int deckIndex = 0; deckIndex < players.Count; deckIndex++)
        {
            // each deck receives 5 cards
            for (int numOfCards = 0; numOfCards < NUM_OF_CARDS_PER_PLAYER; numOfCards++)
            {
                playerDecks[deckIndex].AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
            }

            // check that they do not start with 4 of a kind
            StandardCardRank? rank;
            while ((rank = CheckIfDeckHas4OfAKind(playerDecks[deckIndex])) != null)
            {
                for (int x = 0; x < playerDecks[deckIndex].GetCardCount(); x++)
                {
                    StandardCard card = (StandardCard)playerDecks[deckIndex].GetCard(x);
                    List<Card> fourOfAKind = playerDecks[deckIndex].GetAllCardsOfSpecificRank(card.GetRank());
                    // remove one of those 4 of a kind
                    playerDecks[deckIndex].RemoveCard(fourOfAKind[0]);
                    GetDeck(DeckChoices.UNDEALT).AddCard(fourOfAKind[0]);
                    // add in a new card
                    playerDecks[deckIndex].AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
                }
            }
        }

        // send the cards to the players
        for (int x = 0; x < players.Count; x++)
        {
            PhotonScripts.NetworkController.SendCardsToPlayer(players[x].username, playerDecks[x].GetAllCards());
        }
        // now all players will be sent 5 cards that do not contain a 4 of a kind

        SendOutPlayerTurnIndex();
    }

    /// <summary>
    /// This method is used to verify that the Card that the player wants to play is valid.
    /// It does NOT actually play the card, it only checks if it is possible
    /// </summary>
    /// <param name="cardToPlay"></param>
    /// <returns>True or false depending on validity of move</returns>
    public override bool VerifyMove(Card cardToPlay)
    {
        
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
        //GetPlayerOfCurrentTurn().cards.RemoveCard(cardToPlay);
        StandardCard card = (StandardCard)cardToPlay;
        PlayerInfo playerToQuery = GetPlayer(playerIndex);
        PlayerInfo currentPlayer = GetPlayerOfCurrentTurn();

        if (DoesPlayerHaveCard(playerToQuery.username, card.GetRank()))
        {
            // the did not have any cards
            AdvanceTurn(true);
            return false;
        }
        List<Card> stolenCards = QueryPlayerForCards(playerToQuery.username, card.GetRank());

        // take cards from queried player
        PhotonScripts.NetworkController.RemoveCardsFromPlayer(playerToQuery.username, currentPlayer.username, stolenCards);

        // give cards to current player
        PhotonScripts.NetworkController.SendCardsToPlayer(currentPlayer.username, stolenCards);

        // check if they have 4 of the current rank. If they do, add a point to them and remove those 4 from their deck
        List<Card> fourOfAKind = QueryPlayerForCards(currentPlayer.username, card.GetRank());
        if (fourOfAKind.Count == 4)
        {
            currentPlayer.score++;
            // TODO update UI for score

            // remove the cards from fourOfAKind from the player
            PhotonScripts.NetworkController.RemoveCardsFromPlayer(currentPlayer.username, null, fourOfAKind);
        }

        // do not advance the turn because they stole some cards

        // check if the game has ended
        if (HasGameEnded())
        {
            Debug.Log("Game has ended");
            // TODO clean up game
        }



        return true;
    }

    /// <summary>
    /// Checks if a player has a 4 of a kind of any specific rank of cards
    /// </summary>
    /// <param name="username"></param>
    /// <returns>Returns either null or a StandardCardRank depending on whether it has a 4 of a kind or not</returns>
    public StandardCardRank? CheckIfPlayerHas4OfAKind(string username)
    {
        PlayerInfo player = GetPlayer(username);

        return CheckIfDeckHas4OfAKind(player.cards);
    }


    /// <summary>
    /// Checks if a deck of cards has 4 of a kind in terms of rank
    /// </summary>
    /// <param name="deck"></param>
    /// <returns></returns>
    public StandardCardRank? CheckIfDeckHas4OfAKind(CardDeck deck)
    {
        for (int x = deck.GetCardCount() - 1; x >= 0; x--)
        {
            StandardCard card = (StandardCard)deck.GetCard(x);
            List<Card> fourOfAKind = deck.GetAllCardsOfSpecificRank(card.GetRank());
            if (fourOfAKind.Count == 4)
            {
                // if they do have 4 of a kind
                return card.GetRank();
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if the player has a specific card rank or not
    /// </summary>
    /// <param name="username"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    public bool DoesPlayerHaveCard(string username, StandardCardRank rank)
    {
        return GetPlayer(username).cards.CheckIfCardRankExists(rank);
    }

    /// <summary>
    /// Gets all the cards from a player of a specific username with a specific rank
    /// </summary>
    /// <param name="username"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    public List<Card> QueryPlayerForCards(string username, StandardCardRank rank)
    {
        PlayerInfo playerToCheck = GetPlayer(username);
        if (!playerToCheck.cards.CheckIfCardRankExists(rank))
        {
            return new List<Card>();
        }

        return playerToCheck.cards.GetAllCardsOfSpecificRank(rank);
    }

    /// <summary>
    /// Returns true if the game has reached the end.
    /// </summary>
    /// <returns></returns>
    public bool HasGameEnded()
    {
        List<PlayerInfo> players = GetAllPlayers();

        int cumulativeScore = 0;
        foreach (PlayerInfo player in players)
        {
            cumulativeScore += player.score;
        }

        // 13 total sets need to have been made to end the game
        return (cumulativeScore == 13);
    }
}
