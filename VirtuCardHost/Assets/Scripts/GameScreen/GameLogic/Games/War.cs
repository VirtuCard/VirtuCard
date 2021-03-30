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
    private const int MIN_NUM_OF_PLAYERS = 1;
    private const int MAX_NUM_OF_PLAYERS = 2;
    int playerTurn = 0;
    private static RawImage lastPlayedDeckOne;
    private static RawImage lastPlayedDeckTwo;

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

        SendOutPlayerTurnIndex();
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
            StandardCard toPlay = (StandardCard) GetDeck(DeckChoices.PONEUNPLAYED).GetCard(0);
            GetDeck(DeckChoices.PONEUNPLAYED).RemoveCard(0);
            AddCardToDeck(toPlay, DeckChoices.PONEPLAYED);
            
            Debug.Log("LEFT INIT MOVE " + toPlay.GetSuit().ToString() + "_" + toPlay.GetRank().ToString());

            // updating the texture of the card
            string cardFileName = toPlay.GetSuit().ToString() + "_" + toPlay.GetRank().ToString();
            GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + cardFileName);

            //AdvanceTurn(true);
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

            Debug.Log("LEFT TOP DECK RANK "  + firstCard.GetRank());
            Debug.Log("RIGHT TOP DECK RANK "  + secondCard.GetRank());

            if (firstCard.GetRank() == secondCard.GetRank())
            {
                //AdvanceTurn(true);
                Debug.Log("cards are the same");
            }
            else if (firstCard.GetRank() > secondCard.GetRank())
            {
                Debug.Log("card on left is higher than card on the left");
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
                //AdvanceTurn(true);

                // reset textures since someone has won the deck
                GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
                GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
            }
            else if (firstCard.GetRank() < secondCard.GetRank())
            {
            // Player two has won the turn with a higher ranking card
                Debug.Log("Card on the right is higher than the card on the left");
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
                //AdvanceTurn(true);

                // reset textures since someone has won the deck
                GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
                GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
            } 
        }
        GetDeck(DeckChoices.PONEPLAYED).Print();
        GetDeck(DeckChoices.PTWOPLAYED).Print();
        //GetDeck(DeckChoices.PONEPLAYED)
        //GetDeck(DeckChoices.PONEPLAYED)
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
        return true;
    }
}
