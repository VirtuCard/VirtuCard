using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using ExitGames.Client.Photon;
using PhotonScripts;
using System;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;
using ExitGames.Client.Photon;
using PhotonScripts;
using System;
using Photon.Realtime;

public class War : Game
{
    private const int MIN_NUM_OF_PLAYERS = 2;
    private const int MAX_NUM_OF_PLAYERS = 2;
    int playerTurn = 0;
    private static RawImage lastPlayedDeckOne;
    private static RawImage lastPlayedDeckTwo;

    public static string p1Name;
    public static string p2Name;

    private int firstPlayerIndex;

    public bool firstTurnHappened = true;

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

        List<PlayerInfo> players = GetAllPlayers();

        p1Name = players[0].username;
        p2Name = players[1].username;

        SendOutPlayerTurnIndex();

        firstPlayerIndex = GetCurrentPlayerTurnIndex();
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
    /// This is the method that is called when the player's turn is forcibly skipped by the timer
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void ForceSkipTurn(int playerIndex)
    {

    }

    /// <summary>
    /// Returns the minimum number of players required to start the game
    /// </summary>
    /// <returns></returns>
    public override int GetMinimumNumOfPlayers()
    {
        return MIN_NUM_OF_PLAYERS;
    }

    public override bool VerifyMove(Card cardToPlay)
    {
        return true;
    }

    public override bool VerifyCanSkip(int playerIndex)
    {
        return false;
    }

    /*
    private IENUMERATOR DelayCards()
    {
        yield return new WaitForSeconds(3);

        GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
        GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + "SingleCardBack");

        if (GetDeck(DeckChoices.PONEUNPLAYED).GetCardCount() == 52)
        {
        // declare a winner with raising an event
        object[] content = new object[] { "Player one" };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);

        }
        else if (GetDeck(DeckChoices.PTWOUNPLAYED).GetCardCount() == 52)
        {
        // declare a winner with raising an event
        object[] content = new object[] { "Player two" };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);
        }

    }
    */

     /// <summary>
    /// This method is called when a player plays a card
    /// </summary>
    /// <param name="cardToPlay">Card that the player is playing</param>
    /// <param name="playerIndex">index of the player</param>
    /// <returns></returns>
    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        //bool firstTurnHappened = true;
        if (firstTurnHappened)
        {
            StandardCard toPlay = (StandardCard) GetDeck(DeckChoices.PONEUNPLAYED).GetCard(0);
            GetDeck(DeckChoices.PONEUNPLAYED).RemoveCard(0);
            AddCardToDeck(toPlay, DeckChoices.PONEPLAYED);
            
            Debug.Log("LEFT INIT MOVE " + toPlay.GetSuit().ToString() + "_" + toPlay.GetRank().ToString());

            // updating the texture of the card
            string cardFileName = toPlay.GetSuit().ToString() + "_" + toPlay.GetRank().ToString();
            GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + cardFileName);

            //AdvanceTurn(true);
            //didDoMoveFirst = false;
            firstTurnHappened = false;
        }
        else
        {       
            StandardCard toPlay = (StandardCard) GetDeck(DeckChoices.PTWOUNPLAYED).GetCard(0); 
            GetDeck(DeckChoices.PTWOUNPLAYED).RemoveCard(0); //DeckChoices.PTWOUNPLAYED.GetCardCount()    
            AddCardToDeck(toPlay, DeckChoices.PTWOPLAYED);

            Debug.Log("RIGHT INIT MOVE" + toPlay.GetSuit().ToString() + "_" + toPlay.GetRank().ToString());

            // updating the texture of the card
            string cardFileName = toPlay.GetSuit().ToString() + "_" + toPlay.GetRank().ToString();
            GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + cardFileName);

            StandardCard firstCard = (StandardCard) GetDeck(DeckChoices.PONEPLAYED).GetCard(GetDeck(DeckChoices.PONEPLAYED).GetCardCount() - 1);
            StandardCard secondCard = (StandardCard) GetDeck(DeckChoices.PTWOPLAYED).GetCard(GetDeck(DeckChoices.PTWOPLAYED).GetCardCount() - 1);
            //Debug.Log("LEFT TOP DECK " + firstCard.GetSuit().ToString() + "_" + toPlay.GetRank().ToString());
            //Debug.Log("RIGHT TOP DECK " + secondCard.GetSuit().ToString() + "_" + toPlay.GetRank().ToString());

            Debug.Log("LEFT TOP DECK RANK "  + (int) firstCard.GetRank());
            Debug.Log("RIGHT TOP DECK RANK "  + (int) secondCard.GetRank());

            int first = (int) firstCard.GetRank();
            int second = (int) secondCard.GetRank();

            if (first == second)
            {
                //AdvanceTurn(true);
                Debug.Log("cards are the same");
                HostData.SetDoShowNotificationWindow(true, "Cards are the Same Rank!");
            }
            else if (first > second)
            {
                Debug.Log("card on left is higher than card on the right");
                HostData.SetDoShowNotificationWindow(true, "Left Card is Higher!");
                // Player one has won the turn with a higher ranking card

                // This takes all of player 2's played cards and adds it to player
                // 1's unplayed deck

                /*
                    for (int i = 0; i < GetDeck(DeckChoices.PTWOPLAYED).GetCardCount(); i++)
                    {
                        GetDeck(DeckChoices.PONEUNPLAYED).AddCard(GetDeck(DeckChoices.PTWOPLAYED).PopCard());
                    }
                */
                GetDeck(DeckChoices.PONEUNPLAYED).AddCards(GetDeck(DeckChoices.PTWOPLAYED));
                GetDeck(DeckChoices.PTWOPLAYED).RemoveAllCards();
                // This takes the cards from player 1's played cards and adds it to player
                // 1's unplayed deck
                /*
                for (int i = 0; i < GetDeck(DeckChoices.PONEPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PONEUNPLAYED).AddCard(GetDeck(DeckChoices.PONEPLAYED).PopCard());
                }
                */
                //AdvanceTurn(true);
                GetDeck(DeckChoices.PONEUNPLAYED).AddCards(GetDeck(DeckChoices.PONEPLAYED));
                GetDeck(DeckChoices.PONEPLAYED).RemoveAllCards();

                //StartCoroutine(DelayCards());
                GameScreenController.doFlipWarCards = true;

                // reset textures since someone has won the deck
                
            }
            else if (first < second)
            {
            // Player two has won the turn with a higher ranking card
                Debug.Log("Card on the right is higher than the card on the left");
                HostData.SetDoShowNotificationWindow(true, "Right Card is Higher!");
                // This takes all of the cards from the player ones played deck and adds it
                // to player 2's unplayed deck
                /*
                for (int i = 0; i < GetDeck(DeckChoices.PONEPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PTWOUNPLAYED).AddCard(GetDeck(DeckChoices.PONEPLAYED).PopCard());
                }
                */
                GetDeck(DeckChoices.PTWOUNPLAYED).AddCards(GetDeck(DeckChoices.PONEPLAYED));
                GetDeck(DeckChoices.PONEPLAYED).RemoveAllCards();
                // This takes the cards from player 2's played cards and adds it to player
                // 2's unplayed deck
                /*
                for (int i = 0; i < GetDeck(DeckChoices.PONEPLAYED).GetCardCount(); i++)
                {
                    GetDeck(DeckChoices.PTWOUNPLAYED).AddCard(GetDeck(DeckChoices.PTWOPLAYED).PopCard());
                }
                */
                //AdvanceTurn(true);
                GetDeck(DeckChoices.PTWOUNPLAYED).AddCards(GetDeck(DeckChoices.PTWOPLAYED));
                GetDeck(DeckChoices.PTWOPLAYED).RemoveAllCards();

                // reset textures since someone has won the deck

                //StartCoroutine(DelayCards());
                GameScreenController.doFlipWarCards = true;

                //GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
                //GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
                firstTurnHappened = true;
            } 
            firstTurnHappened = true;
            //didDoMoveFirst = true;
        }
        
        /*
        // checks to see if a player has won
        if (GetDeck(DeckChoices.PONEUNPLAYED).GetCardCount() == 52)
        {
            // declare a winner with raising an event
        object[] content = new object[] { "Player one" };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);

        }
        else if (GetDeck(DeckChoices.PTWOUNPLAYED).GetCardCount() == 52)
        {
            // declare a winner with raising an event
        object[] content = new object[] { "Player two" };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
        */
        AdvanceTurn(true);
        return true;
    
    }
}
