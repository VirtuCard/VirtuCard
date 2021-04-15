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
using System.IO;
using GameScreen.GameLogic.Cards;

public class ClientGameController : MonoBehaviourPunCallbacks
{
    public Button skipBtn;
    public Button playCardBtn;
    public Button drawCardBtn;

    public GameObject errorDisplay;

    // public GameObject turn;
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
    public CanvasGroup chatCanvas;

    // hide/unhide chat in the settings
    public GameObject hideChatPanel;
    public Button hideChatBtn;
    public GameObject unhideChatPanel;
    public Button unhideChatBtn;

    public GameObject warButton;

    // 3 below are used for if the game is over
    public GameObject winnerPanel;
    public Button exitGameBtn;
    public Text winnerAnnounce;
    public GameObject standardPanel;

    public GameObject goFishPanel;
    public Dropdown goFishNamesDropdown;
    public Button goFishQueryButton;

    public CanvasGroup invalidMove;
    public GameObject loadingPanel;

    public Button boilerUp;
    public GameObject animationObject;
    public CanvasGroup animationCanvas;
    public AudioSource BoilerAudio;
    public Button IUSucks;
    public AudioSource IUAudio;

    public List<Card> CardList;

    // this is used to determine if the user has scrolled over to a new card, so it can be used to verify
    private Card previouslySelectedCard;
    bool cardIsValid = false;

    public Timer timer;

    public NotificationWindow notificationWindow;

    public Image boilerCoolDown;
    public Image IUCoolDown;
    public CanvasGroup animationCooldown;
    private bool isCoolDown;
    private float cooldownTimer = -1;
    private float cooldownSeconds = 60;
    public Text boilerCountdown;
    public Text IUCountdown;
    public CanvasGroup boilerAnimation;
    public CanvasGroup IUAnimation;

    private bool wasCurrentlyTurn = false;
    private bool gameOver = false;
    private bool cardsFlipped = false;

    [Header("Card Back Changing")] public Button setCardBackBtn;
    public Button defCardBackBtn;
    public RawImage cardBackImage;
    string filePath;
    public Texture defBack;

    public GameObject selectColor;
    public Button redButton;
    public Button yellowButton;
    public Button greenButton;
    public Button blueButton;

    // Start is called before the first frame update
    void Start()
    {
        defCardBackBtn.interactable = false;
        PhotonNetwork.AddCallbackTarget(this);

        defBack = Resources.Load<Texture>("Card UI/CardBack");

        skipBtn.onClick.AddListener(delegate() { SkipBtnClicked(); });
        playCardBtn.onClick.AddListener(delegate() { PlayCardBtnClicked(); });
        drawCardBtn.onClick.AddListener(delegate() { DrawCardBtnClicked(); });
        SetCanSkipBtn(ClientData.isCurrentTurn());
        cardMenu = cardCarousel.GetComponent<CardMenu>();
        // chat in the settings
        hideChatBtn.onClick.AddListener(delegate() { hideChatSettings(); });
        unhideChatBtn.onClick.AddListener(delegate() { unhideChatSettings(); });

        boilerUp.onClick.AddListener(delegate()
        {
            // boilerUpBtnPressed();
            if (!isCoolDown)
            {
                // Send event
                object[] content = new object[] { };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
                PhotonNetwork.RaiseEvent((int) NetworkEventCodes.BoilerUpEmoji, content, raiseEventOptions,
                    SendOptions.SendUnreliable);
            }
            else
            {
                animationCooldown.GetComponent<CanvasGroup>().alpha = 1;
                StartCoroutine(FadeCanvas(animationCooldown, animationCooldown.alpha, 0, 1.0f));
            }
        });
        IUSucks.onClick.AddListener(delegate()
        {
            if (!isCoolDown)
            {
                // Send event
                object[] content = new object[] { };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
                PhotonNetwork.RaiseEvent((int) NetworkEventCodes.IUSucksEmoji, content, raiseEventOptions,
                    SendOptions.SendUnreliable);
            }
            else
            {
                animationCooldown.GetComponent<CanvasGroup>().alpha = 1;
                StartCoroutine(FadeCanvas(animationCooldown, animationCooldown.alpha, 0, 1.0f));
            }
        });


        // setup timer
        timer.SetupTimer(ClientData.IsTimerEnabled(), ClientData.GetTimerSeconds(), ClientData.GetTimerMinutes(),
            warningThreshold: 30, TimerEarlyWarning, TimerReachedZero);
        if (ClientData.GetGameName() == "GoFish")
        {
            standardPanel.SetActive(false);
            warButton.SetActive(false);
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
        else if (ClientData.GetGameName() == "War")
        {
            warButton.SetActive(true);
            standardPanel.SetActive(false);
            goFishPanel.SetActive(false);
        }
        else
        {
            standardPanel.SetActive(true);
            goFishPanel.SetActive(false);
            warButton.SetActive(false);
        }

        // when winner is announced the button is clicked
        exitGameBtn.onClick.AddListener(delegate() { exitGameBtnOnClick(); });
    }

    // Update is called once per frame
    void Update()
    {
        defBack = Resources.Load<Texture>("Card UI/CardBack");
        // check to see if the player can skip their turn once per frame
        foreach (RectTransform o in cardMenu.images)
        {
            o.Find("Front").GetComponent<Outline>().enabled = false;
        }

        // Remove players that have left game
        goFishNamesDropdown.options.RemoveAll(optionData =>
            !ClientData.GetAllConnectedPlayers().Contains(optionData.text));

        if (ClientData.isCurrentTurn())
        {
            loadingPanel.SetActive(false);
            if (!wasCurrentlyTurn)
            {
                wasCurrentlyTurn = true;
                SetupTurn();
            }

            // turn.SetActive(true);
            notTurnUI.SetActive(false);


            Card selectedCard = cardMenu.GetCurrentlySelectedCard();

            if (selectedCard != null)
            {
                if (previouslySelectedCard == null ||
                    previouslySelectedCard.Compare(selectedCard) == false)
                {
                    // if it is a new card, verify that it is valid
                    VerifyIfCardCanBePlayed(selectedCard);
                    if (ClientData.GetGameName() == "GoFish")
                    {
                        UpdateGoFishButtonText(goFishNamesDropdown.options[goFishNamesDropdown.value].text,
                            ((StandardCard) selectedCard).GetRank());
                    }

                    previouslySelectedCard = selectedCard;
                    cardIsValid = false;
                }

                if (cardIsValid)
                {
                    cardMenu.images[cardMenu.GetCurrentlySelectedIndex()].Find("Front").GetComponent<Outline>()
                        .effectColor = Color.blue;
                }
                else
                {
                    cardMenu.images[cardMenu.GetCurrentlySelectedIndex()].Find("Front").GetComponent<Outline>()
                        .effectColor = Color.red;
                }

                cardMenu.images[cardMenu.GetCurrentlySelectedIndex()].Find("Front").GetComponent<Outline>().enabled =
                    true;
            }
        }
        else
        {
            timer.StopTimer();

            wasCurrentlyTurn = false;

            // turn.SetActive(false);
            // Call the Username of the current player here
            waitingSign.GetComponent<Text>().text = ClientData.getCurrentPlayerTurn() + "'s Turn";
            if (String.IsNullOrEmpty(ClientData.getCurrentPlayerTurn()))
            {
                loadingPanel.SetActive(true);
                waitingSign.GetComponent<Text>().text = "Loading Game...";
            }
            else
            {
                loadingPanel.SetActive(false);
            }

            notTurnUI.SetActive(true);
        }

        // This is where kade and ryan gets stuff from the host
        if (gameOver)
        {
            winnerPanel.SetActive(true);
            winnerAnnounce.GetComponent<Text>().text = "YOU WON!";
        }

        // keep card menu at a valid index
        if (!cardMenu.IsIndexInValidPosition())
        {
            cardMenu.MoveToValidPosition();
        }

        // need this here or chat won't update if host disables char
        updateChat();

        // button cooldown code after the animations have been pressed
        if (isCoolDown)
        {
            boilerCoolDown.fillAmount -= 1 / cooldownSeconds * Time.deltaTime;
            IUCoolDown.fillAmount -= 1 / cooldownSeconds * Time.deltaTime;

            if (boilerCoolDown.fillAmount <= 0)
            {
                IUCoolDown.fillAmount = 0;
                boilerCoolDown.fillAmount = 0;
                isCoolDown = false;
            }
        }

        // countdown text for the animation cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            string timeString = cooldownTimer.ToString();
            timeString = timeString.Substring(0, timeString.IndexOf('.'));
            boilerCountdown.GetComponent<Text>().text = timeString;
            IUCountdown.GetComponent<Text>().text = timeString;
        }
        else
        {
            boilerCountdown.GetComponent<Text>().text = "";
            IUCountdown.GetComponent<Text>().text = "";
        }
    }

    /// <summary>
    /// This is where the player decides if they want to hide the chat or not
    /// If host decides to disable the chat, the chat should disable
    /// </summary>
    public void updateChat()
    {
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
                chatCanvas.GetComponent<CanvasGroup>().alpha = 1;
                hideChatPanel.SetActive(true);
                unhideChatPanel.SetActive(false);
                ClientData.setHideChat(false);

                animationObject.SetActive(true);
                animationCanvas.GetComponent<CanvasGroup>().alpha = 0;
                animationCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else if (chatValue == 1) // hide chat
            {
                dropboxSize.offsetMin = new Vector2(dropboxSize.offsetMin.x, -130);
                dropboxSize.offsetMax = new Vector2(dropboxSize.offsetMax.x, -20);
                chatCanvas.GetComponent<CanvasGroup>().alpha = 0;
                hideChatPanel.SetActive(false);
                unhideChatPanel.SetActive(true);
                ClientData.setHideChat(true);

                animationObject.SetActive(true);
                animationCanvas.GetComponent<CanvasGroup>().alpha = 1;
                animationCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        else
        {
            // chat is disabled from the host
            chatDisableSign.SetActive(true);
            chatCanvas.GetComponent<CanvasGroup>().alpha = 0;
            dropboxUI.SetActive(false);
            animationObject.SetActive(false);
        }
    }

    private void GoFishQueryButtonClicked()
    {
        StandardCard card = (StandardCard) cardMenu.GetCurrentlySelectedCard();
        if (card != null)
        {
            SendCardToHost(card);
        }
        else
        {
            notificationWindow.ShowNotification("Select a Card");
        }
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
                rank = (StandardCardRank) Enum.Parse(typeof(StandardCardRank), currentRank);
            }

            string newText = "Ask " + playerName + " for " + Enum.GetName(typeof(StandardCardRank), rank);
            goFishQueryButton.GetComponentInChildren<Text>().text = newText;
        }
        catch
        {
        }
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
            object[] content = new object[] {PhotonNetwork.NickName, numOfCards};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int) NetworkEventCodes.ClientDrawCard, content, raiseEventOptions,
                SendOptions.SendUnreliable);
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

        drawCardBtn.enabled = playCardBtn.enabled = cardsFlipped;
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
        else
        {
            errorDisplay.GetComponent<Text>().text = "You are not allowed to skip!";
        }
    }

    private void ChangeColorCardPlayed(UnoCardColor color, Card originalCard, int cardIdx)
    {
        selectColor.SetActive(false);
        RemoveCard(originalCard);
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

        UnoCard card = new UnoCard(color, ((UnoCard) originalCard).value);
        SendCardToHost(card);
    }

    private void PlayCardBtnClicked()
    {
        if (ClientData.isCurrentTurn())
        {
            if (cardIsValid)
            {
                Card card = cardMenu.GetCurrentlySelectedCard();
                int cardIdx = cardMenu.GetCurrentlySelectedIndex();

                card.Print();

                if (card.GetType() == typeof(UnoCard))
                {
                    UnoCard currentCard = (UnoCard) card;
                    //If we have a change color card, open up the change color panel to play it.
                    if (currentCard.value == UnoCardValue.WILD || currentCard.value == UnoCardValue.PLUS_FOUR)
                    {
                        redButton.onClick.RemoveAllListeners();
                        yellowButton.onClick.RemoveAllListeners();
                        greenButton.onClick.RemoveAllListeners();
                        blueButton.onClick.RemoveAllListeners();

                        redButton.onClick.AddListener(delegate
                        {
                            ChangeColorCardPlayed(UnoCardColor.RED, card, cardIdx);
                        });
                        yellowButton.onClick.AddListener(delegate
                        {
                            ChangeColorCardPlayed(UnoCardColor.YELLOW, card, cardIdx);
                        });
                        greenButton.onClick.AddListener(delegate
                        {
                            ChangeColorCardPlayed(UnoCardColor.GREEN, card, cardIdx);
                        });
                        blueButton.onClick.AddListener(delegate
                        {
                            ChangeColorCardPlayed(UnoCardColor.BLUE, card, cardIdx);
                        });
                        selectColor.SetActive(true);

                        return;
                    }
                }

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
                invalidMove.GetComponent<CanvasGroup>().alpha = 1;
                StartCoroutine(FadeCanvas(invalidMove, invalidMove.alpha, 0, 1.0f));
                Debug.Log("Card is not valid to be played");
            }
        }
        else
        {
            Debug.Log("Not currently your turn");
        }
    }

    private void exitGameBtnOnClick()
    {
        winnerPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
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
        if (photonEvent.Code == (int) NetworkEventCodes.HostSendInfoToConnectedClient)
        {
            // in JoinGameMethod we don't care about it here
        }
        // this is to see if the card that was sent to be verified was valid
        else if (photonEvent.Code == (int) NetworkEventCodes.HostSendingCardVerification)
        {
            Debug.Log("Receiving Verification");
            object[] data = (object[]) photonEvent.CustomData;
            string username = (string) data[0];
            // ignore if it was not meant for this user
            if (username.Equals(PhotonNetwork.NickName))
            {
                bool isValid = (bool) data[1];
                Debug.Log("Is Valid: " + isValid);
                cardIsValid = isValid;
            }
        }
        // this is the return for the draw card event
        else if (photonEvent.Code == (int) NetworkEventCodes.HostSendingCardsToPlayer)
        {
            Debug.Log("Receiving Card");
            object[] data = (object[]) photonEvent.CustomData;
            string username = (string) data[0];
            // ignore if it was not meant for this user
            if (username.Equals(PhotonNetwork.NickName))
            {
                string cardType = (string) data[1];
                StandardCardRank rank = (StandardCardRank) data[2];
                StandardCardSuit suit = (StandardCardSuit) data[3];
                bool didDrawFromDeck = (bool) data[4];
                bool doShowPlayer = (bool) data[5];
                StandardCard card = new StandardCard(rank, suit);
                card.Print();
                AddCard(card, CardTypes.StandardCard);

                if (doShowPlayer)
                {
                    string displayString = "Received the " + card.ToNiceString();
                    if (didDrawFromDeck)
                    {
                        displayString = "Drew the " + card.ToNiceString();
                    }

                    notificationWindow.ShowNotification(displayString);
                }
            }
        }
        // this is if the host updated the current player turn index
        else if (photonEvent.Code == (int) NetworkEventCodes.UpdatePlayerTurnIndex)
        {
            object[] data = (object[]) photonEvent.CustomData;
            string currentPersonsTurn = (string) data[0];
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
        else if (photonEvent.Code == (int) NetworkEventCodes.HostEnablingTimer)
        {
            object[] data = (object[]) photonEvent.CustomData;
            bool enabled = (bool) data[0];

            // enable/disable timer
            timer.EnableTimer(enabled);
        }
        // remove card event
        else if (photonEvent.Code == (int) NetworkEventCodes.HostRemovingCardsFromPlayer)
        {
            object[] data = (object[]) photonEvent.CustomData;
            string removeFromPlayer = (string) data[0];

            // if it is this player we want to remove cards from
            if (removeFromPlayer.Equals(PhotonNetwork.NickName))
            {
                int numOfCards = (int) data[2];
                Debug.Log("Removing " + numOfCards + " Cards");

                string userWhoTookCards = (string) data[1];

                List<Card> cardsToRemove = new List<Card>();
                for (int x = 3; x < 3 + (numOfCards * 2); x += 2)
                {
                    StandardCard card = new StandardCard((StandardCardRank) data[x], (StandardCardSuit) data[x + 1]);
                    Debug.Log("Removing: " + card.ToString());
                    cardsToRemove.Add(card);
                }

                foreach (Card card in cardsToRemove)
                {
                    RemoveCard(card);
                    notificationWindow.ShowNotification(card.ToNiceString() + " has been taken!");
                }
            }
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.WinnerSelected)
        {
            // This is if a player has been chosen to win
            object[] data = (object[]) photonEvent.CustomData;
            string winnerName = (string) data[0];
            if (winnerName == PhotonNetwork.NickName)
            {
                winnerAnnounce.GetComponent<Text>().text = "You won!";
                ClientData.UserProfile.GamesWon += 1;
                DatabaseUtils.updateUser(ClientData.UserProfile, b => { Debug.Log("Incremented Games won."); });

                winnerPanel.SetActive(true);
            }
            else if (winnerName == "nowinner")
            {
                winnerAnnounce.GetComponent<Text>().text = "Game is over.";
                winnerPanel.SetActive(true);
            }
            else
            {
                winnerAnnounce.GetComponent<Text>().text = winnerName + " Won. Better luck next time!";
                ClientData.UserProfile.GamesLost += 1;
                DatabaseUtils.updateUser(ClientData.UserProfile, b => { Debug.Log("Incremented Games lost."); });

                winnerPanel.SetActive(true);
            }
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.SongVerification) // Music Message
        {
            object[] data = (object[]) photonEvent.CustomData;
            string clientName = (string) data[0];
            bool result = (bool) data[1];
            if (clientName == PhotonNetwork.NickName)
            {
                if (result)
                {
                    notificationWindow.ShowNotification("Song has been added!");
                }
                else
                {
                    notificationWindow.ShowNotification("Song not found!");
                }
            }
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.PlayAgain)
        {
            object[] data = (object[]) photonEvent.CustomData;
            string winnerName = (string) data[0];

            winnerPanel.SetActive(false);

            //TODO CLEAR CARDS
            List<Card> toRemove = cards.GetAllCards();
            int iterate = toRemove.Count;
            for (int i = iterate - 1; i >= 0; i--)
            {
                Debug.Log(toRemove[i]);
                RemoveCard(toRemove[i]);
            }
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.BoilerUpEmoji)
        {
            boilerUpBtnPressed();
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.IUSucksEmoji)
        {
            IUSucksBtnPressed();
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.HostSendingUnoCards)
        {
            Debug.Log("Receiving Uno Card");
            object[] data = (object[]) photonEvent.CustomData;
            string username = (string) data[0];
            // ignore if it was not meant for this user
            if (username.Equals(PhotonNetwork.NickName))
            {
                string cardType = (string) data[1];
                UnoCardColor color = (UnoCardColor) data[2];
                UnoCardValue value = (UnoCardValue) data[3];
                bool didDrawFromDeck = (bool) data[4];
                bool doShowPlayer = (bool) data[5];
                UnoCard card = new UnoCard(color, value);
                card.Print();
                AddCard(card, CardTypes.UnoCard);

                if (doShowPlayer)
                {
                    string displayString = "Received the " + card.ToNiceString();
                    if (didDrawFromDeck)
                    {
                        displayString = "Drew the " + card.ToNiceString();
                    }

                    notificationWindow.ShowNotification(displayString);
                }
            }
        }
        else if (photonEvent.Code == (int) NetworkEventCodes.PlayerKicked)
        {
            object[] data = (object[]) photonEvent.CustomData;
            string kickedPlayerName = (string) data[0];
            if (kickedPlayerName == PhotonNetwork.NickName)
            {
                PhotonNetwork.LeaveRoom();
                ClientData.setJoinCode(null);
                SceneManager.LoadScene(SceneNames.JoinGamePage);
            }

            JoinGameMethod.makeKickedError = true;
        }
    }

    /// <summary>
    /// hide and unhide chat in the settings for the next two functions
    /// </summary>
    public void hideChatSettings()
    {
        chatOptions.value = 1;
        updateChat();
        hideChatPanel.SetActive(false);
        unhideChatPanel.SetActive(true);
    }

    public void unhideChatSettings()
    {
        chatOptions.value = 0;
        updateChat();
        hideChatPanel.SetActive(true);
        unhideChatPanel.SetActive(false);
    }

    /// <summary>
    /// This method is called when the Change card back button is clicked
    /// </summary>
    public void UploadButtonClicked()
    {
        filePath = ""; //EditorUtility.OpenFilePanel("Select your custom card back", "", "png,jpg,jpeg,");
        //setCardBack = true;
/*        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                filePath = path;
            }
        }, "Select a custom card back image", "image/*");*/
        //filePath = EditorUtility.OpenFilePanel("Select your custom card back", "", "png,jpg,jpeg,");

        Debug.Log(filePath);
        // StartCoroutine(PickImage());

        /*cardMenu.backPath = filePath;
        if (filePath.Length != 0)
        {
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            foreach (RectTransform o in cardMenu.images)
            {
                o.Find("Back").GetComponent<RawImage>().texture = tex;
            }
        }*/

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                filePath = path;
                UpdateImage();
            }
        }, "Select a custom card back image", "image/*");
        Debug.Log("Permission result: " + permission);
    }

    private void UpdateImage()
    {
        cardMenu.backPath = filePath;
        if (filePath.Length != 0)
        {
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }

            foreach (RectTransform o in cardMenu.images)
            {
                o.Find("Back").GetComponent<RawImage>().texture = tex;
            }

            defCardBackBtn.interactable = true;
        }

        //      setCardBack = false;
        defCardBackBtn.interactable = true;
    }

    private IEnumerator PickImage()
    {
        yield return new WaitForSeconds(1);

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                filePath = path;
            }
        }, "Select a custom card back image", "image/*");
        cardMenu.backPath = filePath;
        if (filePath.Length != 0)
        {
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }

            foreach (RectTransform o in cardMenu.images)
            {
                o.Find("Back").GetComponent<RawImage>().texture = tex;
            }

            defCardBackBtn.interactable = true;
        }

        //      setCardBack = false;
        defCardBackBtn.interactable = true;
        Debug.Log("Permission result: " + permission);
    }

    /// <summary>
    /// This method is called when the Default card back button is clicked
    /// </summary>
    public void DefButtonClicked()
    {
        Debug.Log("Default Button pressed");
        // StartCoroutine(SetDefCardBack());
        filePath = "";
        cardMenu.backPath = "";
        foreach (RectTransform o in cardMenu.images)
        {
            o.Find("Back").GetComponent<RawImage>().texture = defBack;
        }

//        setCardBack = true;
        defCardBackBtn.interactable = false;
    }

    public IEnumerator SetDefCardBack()
    {
        yield return new WaitForSeconds(1);
        foreach (RectTransform o in cardMenu.images)
        {
            o.Find("Back").GetComponent<RawImage>().texture = Resources.Load<Texture>("Card UI/CardBack");
        }
    }


    /// <summary>
    /// This is the code to update the fade in or fade out for the Canvas Group
    /// </summary>
    public IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float lerpTime) // = 1.0f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            cg.alpha = currentValue;
            if (percentageComplete >= 1) break;
            yield return new WaitForEndOfFrame();
        }
    }

    private void boilerUpBtnPressed()
    {
        BoilerAudio.Play();
        IUCoolDown.fillAmount = 1;
        boilerCoolDown.fillAmount = 1;
        cooldownTimer = 60;
        isCoolDown = true;
        boilerAnimation.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(FadeCanvas(boilerAnimation, boilerAnimation.alpha, 0, 7.0f));
    }

    private void IUSucksBtnPressed()
    {
        IUAudio.Play();
        IUCoolDown.fillAmount = 1;
        boilerCoolDown.fillAmount = 1;
        cooldownTimer = 60;
        isCoolDown = true;
        IUAnimation.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(FadeCanvas(IUAnimation, IUAnimation.alpha, 0, 7.0f));
    }

    /// <summary>
    /// Sends the command to the host to skip this player's turn
    /// </summary>
    /// <param name="username"></param>
    private void SendSkipTurnToHost(bool didRunOutOfTurnTime = false)
    {
        Debug.Log("Sending Skip Command");
        object[] content = new object[] {PhotonNetwork.NickName, didRunOutOfTurnTime};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.ClientSkipTurn, content, raiseEventOptions,
            SendOptions.SendUnreliable);
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

                StandardCard cardToSend = (StandardCard) card;
                object[] content = new object[]
                {
                    PhotonNetwork.NickName, "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit(),
                    requestFromUsername
                };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
                PhotonNetwork.RaiseEvent((int) NetworkEventCodes.ClientPlayedCard, content, raiseEventOptions,
                    SendOptions.SendUnreliable);
            }
            else
            {
                Debug.Log("Sending Card: " + card.ToString());
                StandardCard cardToSend = (StandardCard) card;
                object[] content = new object[]
                    {PhotonNetwork.NickName, "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit()};
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
                PhotonNetwork.RaiseEvent((int) NetworkEventCodes.ClientPlayedCard, content, raiseEventOptions,
                    SendOptions.SendUnreliable);
            }
        }
        else if (card.GetType() == typeof(UnoCard))
        {
            Debug.Log("Sending Card: " + card.ToString());
            UnoCard cardToSend = (UnoCard) card;
            object[] content = new object[]
                {PhotonNetwork.NickName, "UnoCard", cardToSend.color, cardToSend.value};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int) NetworkEventCodes.ClientPlayedCard, content, raiseEventOptions,
                SendOptions.SendUnreliable);
        }
    }

    public void FlipCardClicked()
    {
        //TODO will send a signal to host to flip the top card over
        Debug.Log("FlipCardClicked!");
        object[] content = {PhotonNetwork.NickName};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.ClientWarFlipCard, content, raiseEventOptions,
            SendOptions.SendUnreliable);
    }

    private void VerifyIfCardCanBePlayed(Card card)
    {
        if (card.GetType() == typeof(StandardCard))
        {
            Debug.Log("Verifying Card: " + card.ToString());
            StandardCard cardToSend = (StandardCard) card;
            object[] content = new object[]
                {"StandardCard", cardToSend.GetRank(), cardToSend.GetSuit(), PhotonNetwork.NickName};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int) NetworkEventCodes.VerifyClientCard, content, raiseEventOptions,
                SendOptions.SendUnreliable);
        }

        if (card.GetType() == typeof(UnoCard))
        {
            Debug.Log("Verifying Card: " + card.ToString());
            UnoCard cardToSend = (UnoCard) card;
            object[] content = new object[]
                {"UnoCard", cardToSend.color, cardToSend.value, PhotonNetwork.NickName};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int) NetworkEventCodes.VerifyClientCard, content, raiseEventOptions,
                SendOptions.SendUnreliable);
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
        notificationWindow.ShowNotification("Time is up!");
        SendSkipTurnToHost(true);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        ClientData.FromHashtable(propertiesThatChanged);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string otherPlayerNickName = otherPlayer.NickName;
        if (otherPlayerNickName.Equals(ClientData.HostName) &&
            !ClientData.GetAllConnectedPlayers().Contains(otherPlayerNickName))
        {
            EndGame();
        }
        else
        {
            ClientData.RemoveConnectedPlayerName(otherPlayer.NickName);
        }
    }

    public void EndGame()
    {
        winnerAnnounce.GetComponent<Text>().text = "Game is over.";
        winnerPanel.SetActive(true);
    }
}