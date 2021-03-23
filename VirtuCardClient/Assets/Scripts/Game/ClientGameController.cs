using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

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
    public GameObject dropboxUI;
    public RectTransform dropboxSize;
    public GameObject chatPanel;

    // 3 below are used for if the game is over
    public GameObject winnerPanel;
    public Button exitGameBtn;
    public Text winnerAnnounce;
    public GameObject standardPanel;

    public GameObject goFishPanel;
    public Dropdown goFishNamesDropdown;
    public Button goFishQueryButton;

    public List<Card> CardList;

    // this is used to determine if the user has scrolled over to a new card, so it can be used to verify
    private Card previouslySelectedCard;
    bool cardIsValid = false;
    public Text cardIsValidText;

    public Timer timer;

    private bool wasCurrentlyTurn = false;
    private bool gameOver = false;
    private bool cardsFlipped = false;

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

        // setup timer
        timer.SetupTimer(ClientData.IsTimerEnabled(), ClientData.GetTimerSeconds(), ClientData.GetTimerMinutes(), warningThreshold: 30, TimerEarlyWarning, TimerReachedZero);
        if (ClientData.GetGameName() == "GoFish")
        {
            standardPanel.SetActive(false);
            goFishPanel.SetActive(true);

            List<string> allPlayers = ClientData.GetAllConnectedPlayers();
            foreach (string playerName in allPlayers)
            {
                // only add the name if it is not this person
                if (!playerName.Equals(PhotonNetwork.NickName))
                {
                    goFishNamesDropdown.options.Add(new Dropdown.OptionData(playerName));
                }
            }
            goFishNamesDropdown.onValueChanged.AddListener(GoFishNamesDropdownValueChanged);

            goFishQueryButton.onClick.AddListener(GoFishQueryButtonClicked);
        }
        else
        {
            standardPanel.SetActive(true);
            goFishPanel.SetActive(false);
        }
    }

    private void IncrementGamesPlayed()
    {
        ClientData.UserProfile.GamesPlayed += 1;
        DatabaseUtils.updateUser(ClientData.UserProfile, b => {Debug.Log("Added game to total count");});
    }

    // Update is called once per frame
    void Update()
    {
        // check to see if the player can skip their turn once per frame
        foreach(RectTransform o in cardMenu.images)
        {
            o.Find("RawImage").GetComponent<Outline>().enabled = false;
        }
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
            //cardMenu.images[cardMenu.GetCurrentlySelectedIndex()].Find("RawImage").GetComponent<Outline>().enabled = true;

            if (selectedCard != null)
            {
                cardMenu.images[cardMenu.GetCurrentlySelectedIndex()].Find("RawImage").GetComponent<Outline>().enabled = true;
                if (previouslySelectedCard == null ||
                    previouslySelectedCard.Compare(selectedCard) == false)
                {
                    // if it is a new card, verify that it is valid
                    VerifyIfCardCanBePlayed(selectedCard);
                    UpdateGoFishButtonText(goFishNamesDropdown.options[goFishNamesDropdown.value].text, selectedCard.GetRank());
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
            if (String.IsNullOrEmpty(ClientData.getCurrentPlayerTurn()))
            {
                waitingSign.GetComponent<Text>().text = "Loading Game...";
            }
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
            }

    /// <summary>
    /// This is where the player decides if they want to hide the chat or not
    /// If host decides to disable the chat, the chat should disable
    /// </summary>
    public void updateChat() {
        Debug.Log("chat allowed status: " + ClientData.isChatAllowed());
        int chatValue = chatOptions.value;

        if (ClientData.isChatAllowed())
        {
            // chat is allowed from the host
            chatDisableSign.SetActive(false);
            dropboxUI.SetActive(true);
            if (chatValue == 0) // normal chat
            {
                dropboxSize.offsetMin = new Vector2(dropboxSize.offsetMin.x, 950);
                dropboxSize.offsetMax = new Vector2(dropboxSize.offsetMax.x, 1040);
                chatPanel.SetActive(true);
            }
            else if (chatValue == 1) // hide chat
            {
                dropboxSize.offsetMin = new Vector2(dropboxSize.offsetMin.x, -1130);
                dropboxSize.offsetMax = new Vector2(dropboxSize.offsetMax.x, -1040);
                chatPanel.SetActive(false);
            }

        }
        else
        {
            // chat is not allowed from the host
            chatDisableSign.SetActive(true);
            chatPanel.SetActive(false);
            dropboxUI.SetActive(false);
        }
    }

    private void GoFishQueryButtonClicked()
    {
        StandardCard card = (StandardCard)cardMenu.GetCurrentlySelectedCard();
        SendCardToHost(card);
    }

    /// <summary>
    /// This updates the gofish button text with a new playername and rank.
    /// If either is null, it uses the previous text there instead of an empty string
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="rank"></param>
    private void UpdateGoFishButtonText(string playerName, StandardCardRank? rank)
    {
        string currentText = goFishQueryButton.GetComponentInChildren<Text>().text;

        try
        {
            if (string.IsNullOrEmpty(playerName))
            {
                string currentName = currentText.Substring(currentText.IndexOf(' ') + 1, currentText.IndexOf(" for "));
                playerName = currentName;
            }
            if (rank == null)
            {
                string currentRank = currentText.Substring(currentText.IndexOf(" for ") + 5);
                rank = (StandardCardRank)Enum.Parse(typeof(StandardCardRank), currentRank);
            }
            string newText = "Ask " + playerName + " for " + Enum.GetName(typeof(StandardCardRank), rank);
            goFishQueryButton.GetComponentInChildren<Text>().text = newText;
        } catch { }
    }

    /// <summary>
    /// This handler is called every time the gofish player choice dropdown is changed
    /// </summary>
    public void GoFishNamesDropdownValueChanged(int state)
    {
        string nameSelected = goFishNamesDropdown.options[state].text;
        UpdateGoFishButtonText(nameSelected, null);
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
        cardMenu.AddCardToCarousel(newCard, whichCardType);
        cards.AddCard(newCard);
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

    public void onFlipButtonClicked()
    {
        string animation = cardsFlipped ? "CardUnflipClient" : "CardFlipClient";
        foreach (RectTransform o in cardMenu.images)
        {
            o.GetComponent<Animator>().Play(animation);
        }
        cardsFlipped = !cardsFlipped;
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
        // remove card event
        else if (photonEvent.Code == 13)
        {
            object[] data = (object[])photonEvent.CustomData;
            string removeFromPlayer = (string)data[0];

            // if it is this player we want to remove cards from
            if (removeFromPlayer.Equals(PhotonNetwork.NickName))
            {
                int numOfCards = (int)data[2];
                Debug.Log("Removing " + numOfCards + " Cards");

                string userWhoTookCards = (string)data[1];

                List<Card> cardsToRemove = new List<Card>();
                for (int x = 3; x < 3 + (numOfCards * 2); x += 2)
                {
                    StandardCard card = new StandardCard((StandardCardRank)data[x], (StandardCardSuit)data[x + 1]);
                    Debug.Log("Removing: " + card.ToString());
                    cardsToRemove.Add(card);
                }

                foreach (Card card in cardsToRemove)
                {
                    RemoveCard(card);
                }
            }
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
            if (ClientData.GetGameName().Equals("GoFish"))
            {
                // if the game is gofish, do special handling
                string currentText = goFishQueryButton.GetComponentInChildren<Text>().text;
                string requestFromUsername = goFishNamesDropdown.options[goFishNamesDropdown.value].text;

                StandardCard cardToSend = (StandardCard)card;
                object[] content = new object[] { PhotonNetwork.NickName, "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit(), requestFromUsername };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendUnreliable);
            }
            else
            {
                Debug.Log("Sending Card: " + card.ToString());
                StandardCard cardToSend = (StandardCard)card;
                object[] content = new object[] { PhotonNetwork.NickName, "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit() };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendUnreliable);
            }
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
