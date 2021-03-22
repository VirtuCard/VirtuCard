using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public abstract class Game
{
    private int playerTurnIndex = 0;
    private CardDeck playedCards = new CardDeck();
    private CardDeck undealtCards = new CardDeck();
    private CardDeck ponePlayed = new CardDeck();
    private CardDeck ptwoPlayed = new CardDeck();
    private CardDeck poneUnplayed = new CardDeck();
    private CardDeck ptwoUnplayed = new CardDeck();
    private List<PlayerInfo> players = new List<PlayerInfo>();
    private string gameName;

    /// <summary>
    /// This is the constructor template for all objects of type Game
    /// </summary>
    public Game()
    {
    }

    /// <summary>
    /// Initializes the game once the players have all joined
    /// </summary>
    public abstract void InitializeGame();

    /// <summary>
    /// Returns the name of the game
    /// </summary>
    /// <returns></returns>
    public string GetGameName()
    {
        return gameName;
    }


    /// <summary>
    /// Advances the playerTurnIndex by <paramref name="skipHowMany"/> either forwards or backwards while keeping it within the bounds of the array
    /// </summary>
    /// <param name="forwards">True for forward, false for backward</param>
    /// <param name="skipHowMany">How many players to skip before asssigning whose turn it is</param>
    public void AdvanceTurn(bool forwards, int skipHowMany)
    {
        if (forwards)
        {
            playerTurnIndex += skipHowMany;
            while (playerTurnIndex >= players.Count)
            {
                playerTurnIndex -= players.Count;
            }
            SendOutPlayerTurnIndex();
            return;
        }
        else
        {
            playerTurnIndex -= skipHowMany;
            while (playerTurnIndex < 0)
            {
                playerTurnIndex += players.Count;
            }
            SendOutPlayerTurnIndex();
            return;
        }
    }

    /// <summary>
    /// Advances the playerTurnIndex by 1 either forwards or backwards while keeping it within the bounds of the array
    /// </summary>
    /// <param name="forwards">True for forward, false for backward</param>
    public void AdvanceTurn(bool forwards)
    {
        if (forwards)
        {
            playerTurnIndex++;
            if (playerTurnIndex >= players.Count)
            {
                playerTurnIndex = 0;
            }
        }
        else
        {
            playerTurnIndex--;
            if (playerTurnIndex < 0)
            {
                playerTurnIndex = players.Count - 1;
            }
        }
        SendOutPlayerTurnIndex();
    }

    /// <summary>
    /// This sends out the playerTurnIndex to all the connected Clients
    /// </summary>
    public void SendOutPlayerTurnIndex()
    {
        PlayerInfo currentPlayer = GetPlayerOfCurrentTurn();
        Debug.Log("Setting current turn to " + currentPlayer.photonPlayer.NickName + "'s turn");
        object[] content = new object[] { currentPlayer.photonPlayer.NickName };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(9, content, raiseEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// Gets the player index from a player object.
    /// Returns the index on success, but -1 on failure.
    /// </summary>
    /// <param name="player">The player object to get the player index of</param>
    /// <returns></returns>
    public int GetPlayerIndex(PlayerInfo player)
    {
        return GetPlayerIndex(player.username);
    }

    /// <summary>
    /// Returns the PlayerInfo with the username that matches <paramref name="username"/>
    /// </summary>
    /// <param name="username">Username of the desired player</param>
    /// <returns></returns>
    public PlayerInfo GetPlayer(string username)
    {
        foreach (PlayerInfo player in players)
        {
            if (player.username.Equals(username))
            {
                return player;
            }
        }
        throw new Exception("Could not find player with username " + username);
    }

    /// <summary>
    /// Returns the PlayerInfo with the index that matches <paramref name="playerIndex"/>
    /// </summary>
    /// <param name="playerIndex">Index of the desired player</param>
    /// <returns></returns>
    public PlayerInfo GetPlayer(int playerIndex)
    {
        if (playerIndex < players.Count && playerIndex >= 0)
        {
            return players[playerIndex];
        }
        throw new Exception("Could not find player at index " + playerIndex);
    }

    /// <summary>
    /// Returns a list of all the playerInfos of the connected players
    /// </summary>
    /// <returns></returns>
    public List<PlayerInfo> GetAllPlayers()
    {
        return players;
    }

    /// <summary>
    /// Returns the number of connected players
    /// </summary>
    /// <returns></returns>
    public int GetNumOfPlayers()
    {
        return players.Count;
    }

    /// <summary>
    /// Gets the player index from a player's username.
    /// Returns the index on success, but -1 on failure.
    /// </summary>
    /// <param name="username">The username of the player to get the player index of</param>
    /// <returns></returns>
    public int GetPlayerIndex(string username)
    {
        for (int x = 0; x < players.Count; x++)
        {
            if (username.Equals(players[x].username))
            {
                return x;
            }
        }
        return -1;
    }

    /*
    /// <summary>
    /// Adds a new player to the game logic.
    /// Does not add the player if the limit has been exceeded.
    /// </summary>
    /// <param name="player">Player to add</param>
    /// <returns>Returns true or false depending on whether the player was added</returns>
    public bool AddPlayer(PlayerInfo player)
    {
        if (players.Count >= HostData.GetMaxNumPlayers())
        {
            players.Add(player);
            return true;
        }
        return false;
    }*/

    /// <summary>
    /// Adds a new player to the game logic.
    /// Does not add the player if the limit has been exceeded.
    /// </summary>
    /// <param name="player">Player to add</param>
    /// <returns>Returns true or false depending on whether the player was added</returns>
    public bool AddPlayer(Photon.Realtime.Player player)
    {
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.username = player.NickName;
        playerInfo.score = 0;
        playerInfo.photonPlayer = player;
        playerInfo.cards = new CardDeck();

        if (players.Count < HostData.GetMaxNumPlayers())
        {
            players.Add(playerInfo);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the index of the current player's turn
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPlayerTurnIndex()
    {
        return playerTurnIndex;
    }

    /// <summary>
    /// Returns the player info of the player whose turn it is
    /// </summary>
    /// <returns></returns>
    public PlayerInfo GetPlayerOfCurrentTurn()
    {
        return players[playerTurnIndex];
    }



    /// <summary>
    /// Disconnects a player from the game and removes them from all game lists.
    /// TODO return their hand to deck
    /// </summary>
    /// <param name="player">Player object of the player to disconnect</param>
    public void DisconnectPlayerFromGame(Photon.Realtime.Player player)
    {
        DisconnectPlayerFromGame(player.NickName);
    }

    /// <summary>
    /// Disconnects a player from the game and removes them from all game lists.
    /// TODO return their hand to deck
    /// </summary>
    /// <param name="username">Username of the player to disconnect</param>
    public void DisconnectPlayerFromGame(string username)
    {
        PlayerInfo playerToDisconnect = GetPlayer(username);
        if (GetPlayerIndex(playerToDisconnect) == GetCurrentPlayerTurnIndex())
        {
            // if it is the disconnecting player's turn advance it by one
            AdvanceTurn(true);
        }
        players.Remove(playerToDisconnect);
    }

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
        else if ((int)whichDeck == 2)
        {
            return ponePlayed;
        }
        else if ((int) whichDeck == 3)
        {
            return poneUnplayed;
        }
        else if ((int) whichDeck == 4)
        {
            return ptwoPlayed;
        }
        else if ((int) whichDeck == 5)
        {
            return ptwoUnplayed;
        }

        throw new Exception("Invalid deck specified: " + (int)whichDeck + ". Game.cs:79");
    }

    /// <summary>
    /// Prints all the connected player's information
    /// </summary>
    /// <param name="player"></param>
    public void PrintAllPlayers()
    {
        Debug.Log("---PRINTING PLAYERS---");
        foreach (PlayerInfo player in players)
        {
            Debug.Log(player.ToString());
        }
    }

    /// <summary>
    /// Prints the player's information
    /// </summary>
    /// <param name="player"></param>
    public void PrintPlayer(PlayerInfo player)
    {
        Debug.Log(player.ToString());
    }

    /// <summary>
    /// Creates a standard deck of 52 cards
    /// </summary>
    /// <returns></returns>
    public CardDeck CreateStandard52Deck()
    {
        CardDeck deck = new CardDeck();
        foreach (StandardCardSuit suit in (StandardCardSuit[])Enum.GetValues(typeof(StandardCardSuit)))
        {
            foreach (StandardCardRank rank in (StandardCardRank[])Enum.GetValues(typeof(StandardCardRank)))
            {
                deck.AddCard(new StandardCard(rank, suit));
            }
        }
        return deck;
    }

    /// <summary>
    /// This method returns the maximum number of players for a game
    /// </summary>
    /// <returns></returns>
    public abstract int GetMaximumNumOfPlayers();

    /// <summary>
    /// This method returns teh minimum number of players for a game
    /// </summary>
    /// <returns></returns>
    public abstract int GetMinimumNumOfPlayers();

    /// <summary>
    /// This method is used to verify that the Card that the player wants to play is valid.
    /// It does NOT actually play the card, it only checks if it is possible
    /// </summary>
    /// <param name="cardToPlay"></param>
    /// <returns>True or false depending on validity of move</returns>
    public abstract bool VerifyMove(Card cardToPlay);

    /// <summary>
    /// This method is used when a player plays a card.
    /// </summary>
    /// <param name="cardToPlay">Card the player plays</param>
    /// <param name="playerIndex">The index of the player making the move</param>
    /// <returns></returns>
    public abstract bool DoMove(Card cardToPlay, int playerIndex);

    /// <summary>
    /// This method is used to verify that the player can skip their turn
    /// It does NOT actually skip their turn, it only checks if it is possible
    /// </summary>
    /// <param name="playerIndex">The index of the player</param>
    /// <returns>True or false depending on validity of skip</returns>
    public abstract bool VerifyCanSkip(int playerIndex);

    protected void SetGameName(string gamename)
    {
        gameName = gamename;
    }
}
