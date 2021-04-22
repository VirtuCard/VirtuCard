using PhotonScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker : Game
{
    // cost per round
    private const int ANTE = 1;

    // score each player starts with at the beginning of the game
    private const int STARTING_SCORE = 10;

    // number of cards a player can replace per round
    private const int REPLACEMENTS_PER_ROUND = 3;

    // num of cards each player is given
    private const int NUM_OF_CARDS_PER_PLAYER = 5;

    // min/max players
    private const int MIN_NUM_OF_PLAYERS = 2;
    private const int MAX_NUM_OF_PLAYERS = 10;

    private List<Card> cardsThatHaveBeenFolded;

    // keep track of score
    private int currentPot;
    private int currentBet;
    private int maxWageringAmount;

    // the index of the person who started the wager
    private int firstPlayerIndex;
    private bool hasWagerChangedSinceFirstPlayerIndex;
    // keeps track of if the wagering in that round has begun
    private bool hasWageringBegun;

    private struct HandStats
    {
        public PokerHands hand;
        public ImportantCards importantCards;
    }
    private struct RoundWinner
    {
        public PlayerInfo user;
        public HandStats winningHand;
    }

    public Poker()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.Poker));
    }

    public override void InitializeGame()
    {
        maxWageringAmount = int.MaxValue;
        // set up intial values
        hasWagerChangedSinceFirstPlayerIndex = false;
        hasWageringBegun = false;
        currentBet = ANTE;
        cardsThatHaveBeenFolded = new List<Card>();

        // set up the deck
        CardDeck deck = CreateStandard52Deck();
        GetDeck(DeckChoices.UNDEALT).AddCards(deck);

        // Give out initial cards
        SendOutCards();

        // setup intial pot and scores
        List<PlayerInfo> players = GetAllPlayers();
        currentPot = players.Count * ANTE;
        foreach (PlayerInfo player in players)
        {
            player.score = STARTING_SCORE - ANTE;
            player.pokerScoreWagered = 1;
            player.pokerHasFolded = false;
            player.pokerReplacementsLeft = REPLACEMENTS_PER_ROUND;
        }

        // send out turn index and initial info
        SendBetInfo();
        SendOutPlayerTurnIndex();
        firstPlayerIndex = GetCurrentPlayerTurnIndex();
    }

    /// <summary>
    /// Replaces the hands of all the players who have not run out of score
    /// </summary>
    private void ReplaceHands()
    {
        List<PlayerInfo> players = GetAllPlayers();
        currentPot = players.Count * ANTE;
        currentBet = ANTE;

        // recreate deck
        CardDeck deck = CreateStandard52Deck();
        GetDeck(DeckChoices.UNDEALT).RemoveAllCards();
        GetDeck(DeckChoices.UNDEALT).AddCards(deck);

        for (int x = 0; x < players.Count; x++)
        {
            // check if the player folded for the round, or if they really are out of points
            if (players[x].pokerHasFolded)
            {
                if (players[x].score - ANTE >= 0)
                {
                    players[x].pokerHasFolded = false;
                    Debug.Log(players[x].username + " has had their hand unfolded since they have chips left");
                }
                else
                {
                    Debug.Log(players[x].username + " has had their hand folded again since they have " + players[x].score + " chips left");
                }
            }

            // if the player has not run out of score, remove their hand before they are given a new one
            if (!players[x].pokerHasFolded)
            {
                // remove the cards from each player
                players[x].pokerScoreWagered = ANTE;
                players[x].score -= ANTE;

                players[x].pokerReplacementsLeft = REPLACEMENTS_PER_ROUND;

                if (players[x].score < 0)
                {
                    players[x].pokerHasFolded = true;
                    players[x].score = 0;
                    Debug.Log(players[x].username + " has run out of poker chips!");
                }
                List<Card> cardsToRemove = new List<Card>();
                cardsToRemove.AddRange(players[x].cards.GetAllCards());
                NetworkController.RemoveCardsFromPlayer(players[x].username, null, cardsToRemove);

                // recycle the cards back into the deck
                //GetDeck(DeckChoices.UNDEALT).AddCards(cardsToRemove);
            }

            // add the folded cards back into the deck
            //GetDeck(DeckChoices.UNDEALT).AddCards(cardsThatHaveBeenFolded);
        }
        //if (GetPlayerOfCurrentTurn().pokerHasFolded)
        //{
        if (NumOfPlayersLeftNotFolded() == 1)
        {
            // end game and give that person the victory
            foreach (PlayerInfo player in players)
            {
                if (!player.pokerHasFolded)
                {
                    Debug.Log(player.username + " has won the game!");
                    GameScreenController.DeclareWinner(player.username, player.username + " is the Winner With " + player.score + " Points!");
                    return;
                }
            }
        }
        else if (NumOfPlayersLeftNotFolded() > 0)
        {
            AdvanceTurnToNextNotFoldedPlayer();
        }
        else
        {
            // Everyone has folded??
            // not sure what to do here
        }
        //}
        UpdateMaxWager();
        SendOutCards();
    }

    /// <summary>
    /// Replaces a card from a player's hand with a new random card from the deck
    /// </summary>
    /// <param name="username"></param>
    /// <param name="card"></param>
    public void ReplaceCard(string username, StandardCard card)
    {
        PlayerInfo player = GetPlayer(username);
        player.cards.RemoveCard(card);
        player.pokerReplacementsLeft--;

        // send a new card
        List<Card> cardToSend = new List<Card>();
        cardToSend.Add(GetDeck(DeckChoices.UNDEALT).PopCard());
        NetworkController.SendCardsToPlayer(username, cardToSend, true, true);

        // input the card back into the deck that the player gave up
        GetDeck(DeckChoices.UNDEALT).AddCard(card);

        SendBetInfo();
    }

    /// <summary>
    /// This is used to update all the clients with the current bets
    /// </summary>
    private void SendBetInfo()
    {
        // set the max wager (don't let people bet more than the lowest person's score)
        UpdateMaxWager();

        List<NetworkController.PokerUsernamesAndScores> keyValues = new List<NetworkController.PokerUsernamesAndScores>();
        List<PlayerInfo> players = GetAllPlayers();

        // assemble all the player's stats and send them out
        for (int x = 0; x < players.Count; x++)
        {
            NetworkController.PokerUsernamesAndScores pokerUsernamesAndScores = new NetworkController.PokerUsernamesAndScores
            {
                playerScore = players[x].score,
                playerScoreWagered = players[x].pokerScoreWagered,
                username = players[x].username,
                replacementsLeft = players[x].pokerReplacementsLeft
            };

            keyValues.Add(pokerUsernamesAndScores);
        }
        NetworkController.SendOutPokerBettingInfo(currentPot, currentBet, maxWageringAmount, keyValues);
    }

    /// <summary>
    /// This is called when a user wagers a bet
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="amountWagered"></param>
    public void WagerBet(int playerIndex, int amountWagered)
    {
        // process the wager
        PlayerInfo playerWhoWagered = GetPlayer(playerIndex);

        // reset boolean for next round
        hasWagerChangedSinceFirstPlayerIndex = false;

        // if the wagered 0
        if (amountWagered == 0)
        {
            HostData.SetDoShowNotificationWindow(true, playerWhoWagered.username + " kept the bet at " + currentBet + " points!");
        }
        // if they have enough score to wager that amount
        else if (playerWhoWagered.score >= amountWagered)
        {
            // wager was updated
            hasWagerChangedSinceFirstPlayerIndex = true;


            // update the current pot
            currentPot += amountWagered;

            // update that player's scores
            playerWhoWagered.score -= amountWagered;
            playerWhoWagered.pokerScoreWagered += amountWagered;

            // the bet raised
            if (currentBet < playerWhoWagered.pokerScoreWagered)
            {
                currentBet = playerWhoWagered.pokerScoreWagered;
                HostData.SetDoShowNotificationWindow(true, playerWhoWagered.username + " raised the wager to " + currentBet + " points!");
            }
            // the bet was matched
            else
            {
                HostData.SetDoShowNotificationWindow(true, playerWhoWagered.username + " matched the wager at " + currentBet + " points!");
            }


            // send out new info to all players
            SendBetInfo();
        }
        else
        {
            // that player doesn't have enough to wager that much
            return;
        }
        AdvanceTurnToNextNotFoldedPlayer();

        // if it has gone through all the players
        if (firstPlayerIndex == GetCurrentPlayerTurnIndex())
        {
            // wager wasn't updated
            if (hasWagerChangedSinceFirstPlayerIndex == false && hasWageringBegun == true && amountWagered == 0)
            {
                // end the wagering and see who won the pot
                EndRound();
                return;
            }
        }

        hasWageringBegun = true;
    }

    /// <summary>
    /// This method is called when a player folds their hand
    /// </summary>
    /// <param name="username"></param>
    public void PlayerFolded(string username)
    {
        PlayerInfo player = GetPlayer(username);
        player.pokerHasFolded = true;

        int index = GetPlayerIndex(username);
        if (index == firstPlayerIndex)
        {
            firstPlayerIndex = GetPlayerIndexOfFirstPlayerNotFolded(index);
        }

        if (NumOfPlayersLeftNotFolded() == 1)
        {
            // end round and give that person the pot
            EndRound();
            return;
        }

        // remove the cards from that player's hand
        List<Card> cardsToRemove = new List<Card>();
        cardsToRemove.AddRange(player.cards.GetAllCards());
        NetworkController.RemoveCardsFromPlayer(player.username, null, cardsToRemove);

        // add those cards to the folded list
        cardsThatHaveBeenFolded.AddRange(cardsToRemove);

        AdvanceTurnToNextNotFoldedPlayer();
    }

    /// <summary>
    /// Gets the first index of the first player who has not folded
    /// </summary>
    /// <returns></returns>
    private int GetPlayerIndexOfFirstPlayerNotFolded(int index)
    {
        List<PlayerInfo> players = GetAllPlayers();
        int originalIndex = index;
        index++;
        if (index >= players.Count)
        {
            index = 0;
        }

        while (originalIndex != index)
        {
            if (!players[index].pokerHasFolded)
            {
                return GetPlayerIndex(players[index].username);
            }

            index++;
            if (index >= players.Count)
            {
                index = 0;
            }
        }
        return index;
    }

    /// <summary>
    /// Advances the turn to the next person that has not folded
    /// </summary>
    private void AdvanceTurnToNextNotFoldedPlayer()
    {
        AdvanceTurn(true);
        if (NumOfPlayersLeftNotFolded() == 1)
        {
            // end round and give that person the pot
            EndRound();
        }
        else if (NumOfPlayersLeftNotFolded() > 0)
        {
            while (GetPlayerOfCurrentTurn().pokerHasFolded)
            {
                AdvanceTurn(true);
            }
        }
        else
        {
            // Everyone has folded??
            // not sure what to do here
        }
    }

    /// <summary>
    /// Returns the number of players that have not folded their hands yet
    /// </summary>
    /// <returns></returns>
    private int NumOfPlayersLeftNotFolded()
    {
        int num = 0;
        List<PlayerInfo> players = GetAllPlayers();
        foreach (PlayerInfo player in players)
        {
            if (!player.pokerHasFolded)
            {
                num++;
            }
        }
        return num;
    }

    /// <summary>
    /// Updates the maximum wagering amount by determining the lowest score of all the active players
    /// </summary>
    private void UpdateMaxWager()
    {
        int min = int.MaxValue;
        List<PlayerInfo> players = GetAllPlayers();

        foreach (PlayerInfo player in players)
        {
            if (!player.pokerHasFolded)
            {
                int playerChips = player.score + player.pokerScoreWagered;
                if (playerChips < min)
                {
                    min = playerChips;
                }
            }
        }
        maxWageringAmount = min;
    }

    /// <summary>
    /// Ends that round, finds the winner, and replaces all hands
    /// </summary>
    private void EndRound()
    {
        RoundWinner winner = FindRoundWinner();

        HostData.SetDoShowNotificationWindow(true, winner.user.username + " won with a " + winner.winningHand.importantCards.HandString());

        GetPlayer(winner.user.username).score += currentPot;

        hasWageringBegun = false;
        hasWagerChangedSinceFirstPlayerIndex = false;

        ReplaceHands();
        SendBetInfo();

        firstPlayerIndex = GetCurrentPlayerTurnIndex();
        //firstPlayerIndex = GetPlayerIndexOfFirstPlayerNotFolded();
        if (firstPlayerIndex < 0)
        {
            firstPlayerIndex = 0;
        }
        Debug.LogError("FirstPlayerIndex = " + firstPlayerIndex);
    }

    /// <summary>
    /// Returns the winner of the round
    /// </summary>
    /// <returns></returns>
    private RoundWinner FindRoundWinner()
    {
        List<PlayerInfo> players = GetAllPlayers();
        List<HandStats> playerStats = new List<HandStats>();

        int highestIndex = 0;

        foreach (PlayerInfo player in players)
        {
            if (player.pokerHasFolded)
            {
                playerStats.Add(new HandStats());
            }
            else
            {
                playerStats.Add(GetHandStats(player.cards));
            }
        }

        // find first index that has not folded
        for (int x = 0; x < players.Count; x++)
        {
            if (!players[x].pokerHasFolded)
            {
                highestIndex = x;
                break;
            }
        }

        if (highestIndex != players.Count - 1)
        {
            int startingIndex = highestIndex + 1;
            for (int x = startingIndex; x < players.Count; x++)
            {
                if (!players[x].pokerHasFolded)
                {
                    if (CompareHands(playerStats[x], playerStats[highestIndex]))
                    {
                        highestIndex = x;
                    }
                }
            }
        }

        RoundWinner winner = new RoundWinner
        {
            user = players[highestIndex],
            winningHand = playerStats[highestIndex]
        };
        return winner;
    }

    /// <summary>
    /// Returns true if <paramref name="handOne"/> is greater than <paramref name="handTwo"/>
    /// </summary>
    /// <param name="handOne"></param>
    /// <param name="handTwo"></param>
    /// <returns></returns>
    private bool CompareHands(HandStats handOne, HandStats handTwo)
    {
        if ((int)handOne.hand > (int)handTwo.hand)
        {
            return true;
        }
        else if ((int)handOne.hand < (int)handTwo.hand)
        {
            return false;
        }
        // they are the same hand
        else
        {
            if (handOne.importantCards.Compare(handTwo.importantCards))
            {
                // handOne is greater
                return true;
            }
            else
            {
                // handTwo is greater
                return false;
            }
        }
    }

    /// <summary>
    /// Returns the stats of the current deck of cards
    /// </summary>
    /// <param name="deck"></param>
    /// <returns></returns>
    private HandStats GetHandStats(CardDeck deck)
    {
        HandStats stats = new HandStats();

        // has 4 of a kind
        if (DoesHaveXOfAKind(deck, 4))
        {
            stats.hand = PokerHands.FOUR_OF_A_KIND;
            FourOfAKind importantCards = new FourOfAKind();

            importantCards.kindRank = ((StandardCard)GetXOfAKindMatchingCards(deck, 4)[0][0]).GetRank();
            CardDeck sorted = deck.SortDeckByRank();
            if ((int)((StandardCard)sorted.GetCard(0)).GetRank() == (int)((StandardCard)sorted.GetCard(0)).GetRank())
            {
                importantCards.fifthRank = ((StandardCard)sorted.GetCard(4)).GetRank();
            }

            stats.importantCards = importantCards;
        }
        // has 3 of a kind
        else if (DoesHaveXOfAKind(deck, 3))
        {
            if (GetXOfAKindMatchingCards(deck, 2).Count == 2)
            {
                // has a full house
                stats.hand = PokerHands.FULL_HOUSE;
                FullHouse importantCards = new FullHouse();

                importantCards.tripleKindRank = ((StandardCard)GetXOfAKindMatchingCards(deck, 3)[0][0]).GetRank();

                List<List<Card>> doubles = GetXOfAKindMatchingCards(deck, 3);
                if ((int)((StandardCard)doubles[0][0]).GetRank() == (int)importantCards.tripleKindRank)
                {
                    importantCards.doubleKindRank = ((StandardCard)doubles[1][0]).GetRank();
                }
                else
                {
                    importantCards.doubleKindRank = ((StandardCard)doubles[0][0]).GetRank();
                }

                stats.importantCards = importantCards;
            }
            else
            {
                // only has 3 of a kind
                stats.hand = PokerHands.THREE_OF_A_KIND;
                ThreeOfAKind importantCards = new ThreeOfAKind();
                importantCards.kindRank = ((StandardCard)GetXOfAKindMatchingCards(deck, 3)[0][0]).GetRank();

                CardDeck sorted = deck.SortDeckByRank();
                for (int x = sorted.GetCardCount() - 1; x >= 0; x--)
                {
                    if ((int)((StandardCard)sorted.GetCard(x)).GetRank() == (int)importantCards.kindRank)
                    {
                        sorted.RemoveCard(x);
                    }
                }
                importantCards.fourthRank = ((StandardCard)sorted.GetCard(1)).GetRank();
                importantCards.fifthRank = ((StandardCard)sorted.GetCard(0)).GetRank();
                stats.importantCards = importantCards;
            }
        }
        // has at least one 2 of a kind
        else if (DoesHaveXOfAKind(deck, 2))
        {
            List<List<Card>> matchingCards = GetXOfAKindMatchingCards(deck, 2);
            
            // one pair
            if (matchingCards.Count == 1)
            {
                stats.hand = PokerHands.ONE_PAIR;
                OnePair importantCards = new OnePair();
                importantCards.pairedRank = ((StandardCard)matchingCards[0][0]).GetRank();

                CardDeck sorted = deck.SortDeckByRank();
                for (int x = sorted.GetCardCount() - 1; x >= 0; x--)
                {
                    if ((int)((StandardCard)sorted.GetCard(x)).GetRank() == (int)importantCards.pairedRank)
                    {
                        sorted.RemoveCard(x);
                    }
                }
                importantCards.thirdRank = ((StandardCard)sorted.GetCard(2)).GetRank();
                importantCards.fourthRank = ((StandardCard)sorted.GetCard(1)).GetRank();
                importantCards.fifthRank = ((StandardCard)sorted.GetCard(0)).GetRank();

                stats.importantCards = importantCards;
            }
            // two pair
            else if (matchingCards.Count == 2)
            {
                stats.hand = PokerHands.TWO_PAIR;
                TwoPair importantCards = new TwoPair();

                if ((int)((StandardCard)matchingCards[0][0]).GetRank() > (int)((StandardCard)matchingCards[1][0]).GetRank())
                {
                    importantCards.highPairedRank = ((StandardCard)matchingCards[0][0]).GetRank();
                    importantCards.lowPairedRank = ((StandardCard)matchingCards[1][0]).GetRank();
                }
                else
                {
                    importantCards.highPairedRank = ((StandardCard)matchingCards[1][0]).GetRank();
                    importantCards.lowPairedRank = ((StandardCard)matchingCards[0][0]).GetRank();
                }

                CardDeck sorted = deck.SortDeckByRank();
                for (int x = sorted.GetCardCount() - 1; x >= 0; x--)
                {
                    if ((int)((StandardCard)sorted.GetCard(x)).GetRank() == (int)importantCards.lowPairedRank ||
                        (int)((StandardCard)sorted.GetCard(x)).GetRank() == (int)importantCards.highPairedRank)
                    {
                        sorted.RemoveCard(x);
                    }
                }
                importantCards.fifthRank = ((StandardCard)sorted.GetCard(0)).GetRank();

                stats.importantCards = importantCards;
            }
        }
        // straight-flush
        else if (deck.IsAFlush() == true && deck.IsAStraight() == true)
        {
            stats.hand = PokerHands.STRAIGHT_FLUSH;
            StraightFlush importantCards = new StraightFlush();
            importantCards.lowestRank = ((StandardCard)deck.SortDeckByRank().GetCard(0)).GetRank();
            stats.importantCards = importantCards;
        }
        // straight
        else if (deck.IsAStraight())
        {
            stats.hand = PokerHands.STRAIGHT;
            Straight importantCards = new Straight();
            importantCards.lowestRank = ((StandardCard)deck.SortDeckByRank().GetCard(0)).GetRank();
            stats.importantCards = importantCards;
        }
        // flush
        else if (deck.IsAFlush())
        {
            stats.hand = PokerHands.FLUSH;
            Flush importantCards = new Flush();
            importantCards.highestToLowestRanks = new List<StandardCardRank>();
            List<Card> highestToLowest = deck.SortDeckByRank().GetAllCards();
            highestToLowest.Reverse();

            for (int x = 0; x < highestToLowest.Count; x++)
            {
                importantCards.highestToLowestRanks.Add(((StandardCard)highestToLowest[x]).GetRank());
            }

            stats.importantCards = importantCards;
        }
        // no pair
        else
        {
            stats.hand = PokerHands.NO_PAIR;
            NoPair importantCards = new NoPair();
            importantCards.highestToLowestRanks = new List<StandardCardRank>();
            List<Card> highestToLowest = deck.SortDeckByRank().GetAllCards();
            highestToLowest.Reverse();

            for (int x = 0; x < highestToLowest.Count; x++)
            {
                importantCards.highestToLowestRanks.Add(((StandardCard)highestToLowest[x]).GetRank());
            }

            stats.importantCards = importantCards;
        }

        return stats;
    }

    /// <summary>
    /// Checks if <paramref name="deck"/> contains <paramref name="howManyCardsOfAKind"/> number of matching cards
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="howManyCardsOfAKind"></param>
    /// <returns></returns>
    private bool DoesHaveXOfAKind(CardDeck deck, int howManyCardsOfAKind)
    {
        for (int x = 0; x < deck.GetCardCount(); x++)
        {
            StandardCard card = (StandardCard)deck.GetCard(x);
            List<Card> matchingCards = deck.GetAllCardsOfSpecificRank(card.GetRank());
            if (matchingCards.Count == howManyCardsOfAKind)
            {
                // if they do have howManyCardsOfAKind cards of a kind
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns a List of List<Card> each of the List<Card> contains a matching kind of cards. 
    /// The outer List contains a list of matching card lists
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="howManyCardsOfAKind"></param>
    /// <returns></returns>
    private List<List<Card>> GetXOfAKindMatchingCards(CardDeck deck, int howManyCardsOfAKind)
    {
        List<List<Card>> xOfAKindCards = new List<List<Card>>();
        for (int x = 0; x < deck.GetCardCount(); x++)
        {
            StandardCard card = (StandardCard)deck.GetCard(x);
            List<Card> matchingCards = deck.GetAllCardsOfSpecificRank(card.GetRank());
            if (matchingCards.Count == howManyCardsOfAKind)
            {
                // if they do have howManyCardsOfAKind cards of a kind

                bool isARepeat = false;
                for (int y = 0; y < xOfAKindCards.Count; y++)
                {
                    // if it has already been found
                    if ((int)((StandardCard)xOfAKindCards[y][0]).GetRank() == (int)((StandardCard)matchingCards[0]).GetRank())
                    {
                        isARepeat = true;
                    }
                }
                if (!isARepeat)
                {
                    List<Card> xOfAKind = new List<Card>();
                    xOfAKind.AddRange(matchingCards);
                    xOfAKindCards.Add(xOfAKind);
                }
            }
        }
        return xOfAKindCards;
    }

    /// <summary>
    /// Sends out NUM_OF_CARDS_PER_PLAYER cards to each player
    /// </summary>
    private void SendOutCards()
    {
        List<PlayerInfo> players = GetAllPlayers();
        List<CardDeck> playerDecks = new List<CardDeck>();

        for (int x = 0; x < players.Count; x++)
        {
            playerDecks.Add(new CardDeck());
        }

        for (int deckIndex = 0; deckIndex < players.Count; deckIndex++)
        {
            // only give cards to players that are not out of score
            if (!players[deckIndex].pokerHasFolded)
            {
                // each deck receives NUM_OF_CARDS_PER_PLAYER cards
                for (int numOfCards = 0; numOfCards < NUM_OF_CARDS_PER_PLAYER; numOfCards++)
                {
                    playerDecks[deckIndex].AddCard(GetDeck(DeckChoices.UNDEALT).PopCard());
                }
            }
        }
        // send the cards to the players
        for (int x = 0; x < players.Count; x++)
        {
            // only give cards to players that are not out of score
            if (!players[x].pokerHasFolded)
            {
                Debug.Log("Sending cards to " + players[x].username);
                NetworkController.SendCardsToPlayer(players[x].username, playerDecks[x].GetAllCards(), true, false);
            }
        }
    }

    /// <summary>
    /// This method should not be called for poker
    /// </summary>
    /// <param name="cardToPlay"></param>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
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
    public int GetCurrentBet()
    {
        return currentBet;
    }
    public int GetCurrentPot()
    {
        return currentPot;
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

public enum PokerHands
{
    NO_PAIR, ONE_PAIR, TWO_PAIR, THREE_OF_A_KIND, STRAIGHT, FLUSH, FULL_HOUSE, FOUR_OF_A_KIND, STRAIGHT_FLUSH
}

public abstract class ImportantCards
{
    public abstract void Print();
    public string GetName(StandardCardRank rank)
    {
        return Enum.GetName(typeof(StandardCardRank), rank);
    }
    /// <summary>
    /// Returns positive if rank1 is greater than rank2
    /// Returns negative if rank1 is less than rank2
    /// Returns 0 if equal
    /// </summary>
    /// <param name="rank1"></param>
    /// <param name="rank2"></param>
    /// <returns></returns>
    public int CompareRanks(StandardCardRank rank1, StandardCardRank rank2)
    {
        if ((int)rank1 > (int)rank2)
        {
            return 1;
        }
        else if ((int)rank1 < (int)rank2)
        {
            return -1;
        }
        return 0;
    }

    public abstract string HandString();

    /// returns true if parameter is less than the current hand
    /// returns false if parameter is greater than current hand
    public abstract bool Compare(ImportantCards handToCompareTo);
}
public class NoPair : ImportantCards
{
    public List<StandardCardRank> highestToLowestRanks;
    public override bool Compare(ImportantCards handToCompareTo)
    {
        NoPair hand = (NoPair)handToCompareTo;
        for (int x = 0; x < highestToLowestRanks.Count; x++)
        {
            if (highestToLowestRanks[x] < hand.highestToLowestRanks[x])
            {
                return false;
            }
            else if (highestToLowestRanks[x] > hand.highestToLowestRanks[x])
            {
                return true;
            }
        }
        return true;
    }

    public override string HandString()
    {
        return "High Card";
    }

    public override void Print()
    {
        String str = string.Empty;
        for (int x = 0; x < highestToLowestRanks.Count; x++)
        {
            str += Enum.GetName(typeof(StandardCardRank), highestToLowestRanks[x]) + ", ";
        }
        str = str.Trim().Trim(',');
        Debug.Log("NoPair Ranks: " + str);
    }
}
public class OnePair : ImportantCards
{
    public StandardCardRank pairedRank;
    public StandardCardRank thirdRank;
    public StandardCardRank fourthRank;
    public StandardCardRank fifthRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        OnePair hand = (OnePair)handToCompareTo;
        int pairedCompare = CompareRanks(pairedRank, hand.pairedRank);
        if (pairedCompare != 0)
        {
            if (pairedCompare > 0)
                return true;
            else
                return false;
        }
        int thirdCompare = CompareRanks(thirdRank, hand.thirdRank);
        if (thirdCompare != 0)
        {
            if (thirdCompare > 0)
                return true;
            else
                return false;
        }
        int fourthCompare = CompareRanks(fourthRank, hand.fourthRank);
        if (fourthCompare != 0)
        {
            if (fourthCompare > 0)
                return true;
            else
                return false;
        }
        int fifthCompare = CompareRanks(fifthRank, hand.fifthRank);
        if (fifthCompare != 0)
        {
            if (fifthCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }

    public override string HandString()
    {
        return "Single Pair";
    }
    public override void Print()
    {
        String str = string.Format("OnePair Ranks: paired: {0}, {1}, {2}, {3}", GetName(pairedRank), GetName(thirdRank), GetName(fourthRank), GetName(fifthRank));
        Debug.Log(str);
    }
}
public class TwoPair : ImportantCards
{
    public StandardCardRank highPairedRank;
    public StandardCardRank lowPairedRank;
    public StandardCardRank fifthRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        TwoPair hand = (TwoPair)handToCompareTo;
        int highPairedCompare = CompareRanks(highPairedRank, hand.highPairedRank);
        if (highPairedCompare != 0)
        {
            if (highPairedCompare > 0)
                return true;
            else
                return false;
        }
        int lowPairedCompare = CompareRanks(lowPairedRank, hand.lowPairedRank);
        if (lowPairedCompare != 0)
        {
            if (lowPairedCompare > 0)
                return true;
            else
                return false;
        }
        int fifthCompare = CompareRanks(fifthRank, hand.fifthRank);
        if (fifthCompare != 0)
        {
            if (fifthCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }

    public override string HandString()
    {
        return "Two Pair";
    }
    public override void Print()
    {
        String str = string.Format("TwoPair Ranks: pairedHigh: {0}, pairedLow {1}, {2}", GetName(highPairedRank), GetName(lowPairedRank), GetName(fifthRank));
        Debug.Log(str);
    }
}
public class ThreeOfAKind : ImportantCards
{
    public StandardCardRank kindRank;
    public StandardCardRank fourthRank;
    public StandardCardRank fifthRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        ThreeOfAKind hand = (ThreeOfAKind)handToCompareTo;
        int kindCompare = CompareRanks(kindRank, hand.kindRank);
        if (kindCompare != 0)
        {
            if (kindCompare > 0)
                return true;
            else
                return false;
        }
        int fourthCompare = CompareRanks(fourthRank, hand.fourthRank);
        if (fourthCompare != 0)
        {
            if (fourthCompare > 0)
                return true;
            else
                return false;
        }
        int fifthCompare = CompareRanks(fifthRank, hand.fifthRank);
        if (fifthCompare != 0)
        {
            if (fifthCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }
    public override string HandString()
    {
        return "Three of a Kind";
    }
    public override void Print()
    {
        String str = string.Format("ThreeKind Ranks: kind: {0}, {1}, {2}", GetName(kindRank), GetName(fourthRank), GetName(fifthRank));
        Debug.Log(str);
    }
}
public class Straight : ImportantCards
{
    public StandardCardRank lowestRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        Straight hand = (Straight)handToCompareTo;
        int lowestCompare = CompareRanks(lowestRank, hand.lowestRank);
        if (lowestCompare != 0)
        {
            if (lowestCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }
    public override string HandString()
    {
        return "Straight";
    }
    public override void Print()
    {
        String str = string.Format("Straight Lowest Rank: {0}", GetName(lowestRank));
        Debug.Log(str);
    }
}
public class Flush : ImportantCards
{
    public List<StandardCardRank> highestToLowestRanks;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        Flush hand = (Flush)handToCompareTo;
        for (int x = 0; x < highestToLowestRanks.Count; x++)
        {
            if (highestToLowestRanks[x] < hand.highestToLowestRanks[x])
            {
                return false;
            }
            else if (highestToLowestRanks[x] > hand.highestToLowestRanks[x])
            {
                return true;
            }
        }
        return true;
    }

    public override string HandString()
    {
        return "Flush";
    }
    public override void Print()
    {
        String str = string.Empty;
        for (int x = 0; x < highestToLowestRanks.Count; x++)
        {
            str += Enum.GetName(typeof(StandardCardRank), highestToLowestRanks[x]) + ", ";
        }
        str = str.Trim().Trim(',');
        Debug.Log("Flush Ranks: " + str);
    }
}
public class FullHouse : ImportantCards
{
    public StandardCardRank tripleKindRank;
    public StandardCardRank doubleKindRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        FullHouse hand = (FullHouse)handToCompareTo;
        int tripleCompare = CompareRanks(tripleKindRank, hand.tripleKindRank);
        if (tripleCompare != 0)
        {
            if (tripleCompare > 0)
                return true;
            else
                return false;
        }
        int doubleCompare = CompareRanks(doubleKindRank, hand.doubleKindRank);
        if (doubleCompare != 0)
        {
            if (doubleCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }
    public override string HandString()
    {
        return "Full House";
    }
    public override void Print()
    {
        String str = string.Format("FullHouse Ranks: triple: {0}, double: {1}", GetName(tripleKindRank), GetName(doubleKindRank));
        Debug.Log(str);
    }
}
public class FourOfAKind : ImportantCards
{
    public StandardCardRank kindRank;
    public StandardCardRank fifthRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        FourOfAKind hand = (FourOfAKind)handToCompareTo;
        int kindCompare = CompareRanks(kindRank, hand.kindRank);
        if (kindCompare != 0)
        {
            if (kindCompare > 0)
                return true;
            else
                return false;
        }
        int fifthCompare = CompareRanks(fifthRank, hand.fifthRank);
        if (fifthCompare != 0)
        {
            if (fifthCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }
    public override string HandString()
    {
        return "Four of a Kind";
    }
    public override void Print()
    {
        String str = string.Format("FourKind Ranks: kind: {0}, {1}", GetName(kindRank), GetName(fifthRank));
        Debug.Log(str);
    }
}
public class StraightFlush : ImportantCards
{
    public StandardCardRank lowestRank;

    public override bool Compare(ImportantCards handToCompareTo)
    {
        StraightFlush hand = (StraightFlush)handToCompareTo;
        int lowestCompare = CompareRanks(lowestRank, hand.lowestRank);
        if (lowestCompare != 0)
        {
            if (lowestCompare > 0)
                return true;
            else
                return false;
        }
        return true;
    }
    public override string HandString()
    {
        return "Straight Flush";
    }
    public override void Print()
    {
        String str = string.Format("StraightFlush Lowest Rank: {0}", GetName(lowestRank));
        Debug.Log(str);
    }
}

