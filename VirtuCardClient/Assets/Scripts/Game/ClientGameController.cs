using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientGameController : MonoBehaviourPunCallbacks
{
    public Button skipBtn;
    public Button playCardBtn;
    public Button drawCardBtn;
    public GameObject errorDisplay;
    public GameObject turn;
    public GameObject notTurnUI;
    public Text waitingSign;
    
    private CardDeck cards = new CardDeck();

    public GameObject cardCarousel;
    private CardMenu cardMenu;
    public GameObject chatDisableSign;

    public Dropdown chatOptions;
    public GameObject chatToggleObject;
    public GameObject chatPanel;
    public GameObject checkMark;
    public Toggle chatToggle;

    // 3 below are used for if the game is over
    public GameObject winnerPanel;
    public Button exitGameBtn;
    public Text winnerAnnounce;

    // this is used to determine if the user has scrolled over to a new card, so it can be used to verify
    private Card previouslySelectedCard;
    bool cardIsValid = false;
    public Text cardIsValidText;

    public Timer timer;

    private bool wasCurrentlyTurn = false;
    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        // ClientData.setChatAllowed(false);

        PhotonNetwork.AddCallbackTarget(this);
        skipBtn.onClick.AddListener(delegate() {
            SkipBtnClicked();
        });
        playCardBtn.onClick.AddListener(delegate () {
            PlayCardBtnClicked();
        });
        drawCardBtn.onClick.AddListener(delegate () { DrawCardBtnClicked(); });
        SetCanSkipBtn(ClientData.isCurrentTurn());
        cardMenu = cardCarousel.GetComponent<CardMenu>();

        chatToggle.SetIsOnWithoutNotify(ClientData.isChatAllowed());
        chatToggle.onValueChanged.AddListener(delegate { ChatToggleValueChanged(chatToggle.isOn); });

        // setup timer
        timer.SetupTimer(ClientData.IsTimerEnabled(), ClientData.GetTimerSeconds(), ClientData.GetTimerMinutes(), warningThreshold: 30, TimerEarlyWarning, TimerReachedZero);
    }

    // Update is called once per frame
    void Update()
    {
        // check to see if the player can skip their turn once per frame
        if (ClientData.isCurrentTurn())
        {
            if (!wasCurrentlyTurn)
            {
                wasCurrentlyTurn = true;
                SetupTurn();
            }
            turn.SetActive(true);
            notTurnUI.SetActive(false);

            StandardCard selectedCard = (StandardCard)cardMenu.GetCurrentlySelectedCard();

            if (selectedCard != null)
            {
                if (previouslySelectedCard == null ||
                    previouslySelectedCard.Compare(selectedCard) == false)
                {
                    // if it is a new card, verify that it is valid
                    VerifyIfCardCanBePlayed(selectedCard);
                    previouslySelectedCard = selectedCard;
                    cardIsValid = false;
                    cardIsValidText.text = "Card is NOT valid";
                }
            }
        }
        else
        {
            timer.StopTimer();

            wasCurrentlyTurn = false;

            turn.SetActive(false);
            // Call the Username of the current player here
            waitingSign.GetComponent<Text>().text = ClientData.getCurrentPlayerTurn() + "'s Turn";
            notTurnUI.SetActive(true);
        }

        // This is where kade and ryan gets stuff from the host
        if (gameOver) {
            winnerPanel.SetActive(true);
            winnerAnnounce.GetComponent<Text>().text = "YOU WON!";

            // if (the current player is the winner) {
            //   winnerAnnounce.GetComponent<Text>().text = "YOU WON!";
            //}
            // // or you didn't win
            // else
            // {
            //   winnerAnnounce.GetComponent<Text>().text = winner's username + " has won the game!";
            // }
            exitGameBtn.onClick.AddListener(delegate () { exitGameBtnOnClick(); });

        }

        updateChat();
        
        disableChat();
    }

    /// <summary>
    /// This is where the player decides if they want to hide the chat or not
    /// </summary>
    public void updateChat() {
        int chatValue = chatOptions.value;
        if (chatValue == 0) // normal chat
        {
            chatPanel.SetActive(true);
        }
        else if (chatValue == 1) // hide chat
        {
            chatPanel.SetActive(false);
        }
    }

    /// <summary>
    /// If host decides to disable the chat, the chat should disable
    /// </summary>
    public void disableChat() {
        Debug.Log("chat allowed status: " + ClientData.isChatAllowed());
        if (ClientData.isChatAllowed())
        {
            // chat is allowed
            chatDisableSign.SetActive(false);
            chatPanel.SetActive(true);
            chatToggleObject.SetActive(true);
        }
        else
        {
            // chat is not allowed
            chatDisableSign.SetActive(true);
            chatPanel.SetActive(false);
            chatToggleObject.SetActive(false);
        }
    }



    /// <summary>
    /// This just adds a random card, which actually is not random
    /// </summary>
    public void AddRandomStandardCard()
    {
        AddCard(new StandardCard(StandardCardRank.FOUR, StandardCardSuit.HEARTS), CardTypes.StandardCard);
        
    }

    /// <summary>
    /// This method is called everytime the draw button is clicked
    /// </summary>
    public void DrawCardBtnClicked()
    {
        if (ClientData.isCurrentTurn())
        {
            // send request for a new card
            int numOfCards = 1;
            object[] content = new object[] { PhotonNetwork.NickName, numOfCards };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(7, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
        else
        {
            Debug.Log("Not currently your turn");
        }
    }

    /// <summary>
    /// Adds a card to both the deck and the card carousel and reformats accordingly
    /// </summary>
    /// <param name="newCard"></param>
    /// <param name="whichCardType"></param>
    public void AddCard(Card newCard, CardTypes whichCardType)
    {
        bool checkTurn = ClientData.isCurrentTurn();
        if (checkTurn){
          cardMenu.AddCardToCarousel(newCard, whichCardType);
          cards.AddCard(newCard);
        }
        else
        {
            Debug.Log("It is not the player's turn!");
        }
    }

    /// <summary>
    /// Removes the card from both the deck and the card carousel and reformats accordingly
    /// </summary>
    /// <param name="cardToRemove"></param>
    public void RemoveCard(Card cardToRemove)
    {
        cards.RemoveCard(cardToRemove);
        cardMenu.RemoveCardFromCarousel(cardToRemove);
    }

    /// <summary>
    /// This method should called every time the game swaps to the player's turn
    /// It should setup all the necessary things like enabling skip btn, timer, etc...
    /// </summary>
    private void SetupTurn()
    {
        SetCanSkipBtn(ClientData.isCurrentTurn());
    }

    /// <summary>
    /// This method sets the skip btn to interactable or not
    /// </summary>
    /// <param name="value">true if the button should be enabled, false otherwise</param>
    private void SetCanSkipBtn(bool value)
    {
        skipBtn.interactable = value;
    }

    /// <summary>
    /// This method is run everythime the skip button is clicked
    /// </summary>
    private void SkipBtnClicked()
    {
        Debug.Log("Skipping...");
        // TODO implementation
        if (GameRules.skipAllowed())
        {
            ClientData.setCurrentTurn(false);
            SetCanSkipBtn(false);
            SendSkipTurnToHost();

        }
        else {
            errorDisplay.GetComponent<Text>().text = "You are not allowed to skip!";
        }
    }

    private void PlayCardBtnClicked()
    {
        if (ClientData.isCurrentTurn())
        {
            if (cardIsValid)
            {
                StandardCard card = (StandardCard)cardMenu.GetCurrentlySelectedCard();
                int cardIdx = cardMenu.GetCurrentlySelectedIndex();
                card.Print();
                RemoveCard(card);
                if (cardIdx > 0)
                {
                    cardMenu.MoveCarouselToIndex(cardIdx - 1);
                }
                else
                {
                    // card was at 0
                    if (cards.GetCardCount() == 0)
                    {
                        // if there are no cards in their hand, don't move carousel
                    }
                    else
                    {
                        // otherwise, do move it
                        cardMenu.MoveCarouselToIndex(0);
                    }
                }
                SendCardToHost(card);
            }
            else
            {
                Debug.Log("Card is not valid to be played");
            }
        }
        else
        {
            Debug.Log("Not currently your turn");
        }
    }

    private void exitGameBtnOnClick() {
        winnerPanel.SetActive(false);
        SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
    }

    /// <summary>
    /// This method is called when the chat toggle state changes
    /// </summary>
    /// <param name="toggleVal"></param>
    private void ChatToggleValueChanged(bool toggleVal)
    {
        checkMark.SetActive(toggleVal);
        chatPanel.SetActive(!toggleVal);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnSignalSent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnSignalSent;
    }

    private void OnSignalSent(EventData photonEvent)
    {
        // Every photon event has its own unique code, I've chosen
        // 1 as the one to work with the initial room information return
        if (photonEvent.Code == 1)
        {
            // in JoinGameMethod we don't care about it here
        }
        // this is to see if the card that was sent to be verified was valid
        else if (photonEvent.Code == 5)
        {
            Debug.Log("Receiving Verification");
            object[] data = (object[])photonEvent.CustomData;
            string username = (string)data[0];
            // ignore if it was not meant for this user
            if (username.Equals(PhotonNetwork.NickName))
            {
                bool isValid = (bool)data[1];
                Debug.Log("Is Valid: " + isValid);
                cardIsValid = isValid;
                if (cardIsValid)
                {
                    cardIsValidText.text = "Card is valid";
                }
                else
                {
                    cardIsValidText.text = "Card is NOT valid";
                }
            }
        }
        // this is the return for the draw card event
        else if (photonEvent.Code == 8)
        {
            Debug.Log("Receiving Card");
            object[] data = (object[])photonEvent.CustomData;
            string username = (string)data[0];
            // ignore if it was not meant for this user
            if (username.Equals(PhotonNetwork.NickName))
            {
                string cardType = (string)data[1];
                StandardCardRank rank = (StandardCardRank)data[2];
                StandardCardSuit suit = (StandardCardSuit)data[3];
                StandardCard card = new StandardCard(rank, suit);
                card.Print();
                AddCard(card, CardTypes.StandardCard);
            }
        }
        // this is if the host updated the current player turn index
        else if (photonEvent.Code == 9)
        {
            object[] data = (object[])photonEvent.CustomData;
            string currentPersonsTurn = (string)data[0];
            Debug.Log("Setting CurrentTurn: " + currentPersonsTurn);
            ClientData.setCurrentPlayerTurn(currentPersonsTurn);
            if (currentPersonsTurn.Equals(PhotonNetwork.NickName))
            {
                ClientData.setCurrentTurn(true);
                timer.StartTimer();
            }
            else
            {
                ClientData.setCurrentTurn(false);
            }
        }
        // this is if the host is either enabling or disabling the timer during the middle of the game
        else if (photonEvent.Code == 11)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool enabled = (bool)data[0];

            // enable/disable timer
            timer.EnableTimer(enabled);
        }
    }

    /// <summary>
    /// Sends the command to the host to skip this player's turn
    /// </summary>
    /// <param name="username"></param>
    private void SendSkipTurnToHost(bool didRunOutOfTurnTime = false)
    {
        Debug.Log("Sending Skip Command");
        object[] content = new object[] { PhotonNetwork.NickName, didRunOutOfTurnTime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(10, content, raiseEventOptions, SendOptions.SendUnreliable);
    }

    /// <summary>
    /// Sends the card to the card to the host
    /// </summary>
    /// <param name="card"></param>
    private void SendCardToHost(Card card)
    {
        if (card.GetType().Name == "StandardCard")
        {
            Debug.Log("Sending Card: " + card.ToString());
            StandardCard cardToSend = (StandardCard)card;
            object[] content = new object[] { PhotonNetwork.NickName, "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit() };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }

    private void VerifyIfCardCanBePlayed(Card card)
    {
        if (card.GetType().Name == "StandardCard")
        {
            Debug.Log("Verifying Card: " + card.ToString());
            StandardCard cardToSend = (StandardCard)card;
            object[] content = new object[] { "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit(), PhotonNetwork.NickName };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
    
    /// <summary>
    /// This method is called every time the timer crosses the early warning threshold
    /// </summary>
    private void TimerEarlyWarning()
    {
        Debug.Log("Hurry up, only " + timer.GetEarlyWarningThreshold() + " seconds left");
    }

    /// <summary>
    /// This method is called every time the timer reaches 0
    /// </summary>
    private void TimerReachedZero()
    {
        SendSkipTurnToHost(true);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        ClientData.FromHashtable(propertiesThatChanged);
    }
}
