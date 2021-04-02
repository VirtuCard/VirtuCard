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

public class GameScreenController : MonoBehaviour
{
    public NotificationWindow notificationWindow;

    public GameObject allOfChatUI;
    public GameObject chatPanel;
    public CanvasGroup chatCanvas;
    public GameObject MessageContent;
    public Text currentPlayer;

    public GameObject settingsPanel;

    public Dropdown chatOptions;
    public RectTransform chatPlace;
    public Toggle timerToggle;

    public GameObject winnerPanel;
    public Dropdown winnerDropdown;
    public GameObject gameOverPanel;
    public GameObject gameOverText;
    public GameObject endGamePanel;

    public GameObject warPanel;
    public GameObject goFishPanel;
    public GameObject standardPanel;

    public List<GameObject> goFishDeckCardsUI;

    public Timer timer;

    public GameObject playedCardCarousel;
    public GameObject undealtCardCarousel;
    private CardMenu playedCardMenu;
    private CardMenu undealtCardMenu;

    public PlayerList playerUIList;

    public RawImage lastPlayedCard;

    //War last played cards
    public RawImage lastPlayedDeckOne;
    public RawImage lastPlayedDeckTwo;
    public static Texture textureOne;
    public static Texture textureTwo;

    public Button DeclareWinnerButton;

    private bool hasInitializedGame = false;

    private float startTime;

    // this is the time in seconds before the game intitializes
    private int secondsBeforeInitialization = 2;


    private static bool isDeclaringWinner = false;
    private static bool isGameEnded = true;

    public static bool doFlipWarCards = false;

    public Button shuffleButton;


    // Start is called before the first frame update
    void Start()
    {
        isDeclaringWinner = false;
        isGameEnded = true;
        if (HostData.isChatAllowed())
        {
            allOfChatUI.SetActive(true);

            // check if chat is muted from the waiting setting screen
            if (HostData.isChatMute()) {
                chatOptions.value = 2;
                updatingChat();
            }
        }
        else
        {
            allOfChatUI.SetActive(false);
        }

        startTime = Time.time;
        playedCardMenu = playedCardCarousel.GetComponent<CardMenu>();
        undealtCardMenu = undealtCardCarousel.GetComponent<CardMenu>();

        // setup settings menu
        chatOptions.onValueChanged.AddListener(e => updatingChat());
        settingsPanel.SetActive(false);
        timerToggle.SetIsOnWithoutNotify(HostData.IsTimerEnabled());
        timerToggle.onValueChanged.AddListener(delegate { EnableTimer(timerToggle.isOn); });
        timerToggle.gameObject.SetActive(HostData.IsTimerEnabled());

        // setup timer
        timer.SetupTimer(HostData.IsTimerEnabled(), HostData.GetTimerSeconds(), HostData.GetTimerMinutes(),
            warningThreshold: 30, TimerEarlyWarning, TimerReachedZero);

        if (HostData.isFreeplay() && !HostData.getDisplayLastCard())
        {
            //Hide played card deck here.
            playedCardCarousel.SetActive(false);
        }

        // add in the player list UI
        List<PlayerInfo> connectedPlayers = HostData.GetGame().GetAllPlayers();
        foreach (var connectedPlayer in connectedPlayers)
        {
            playerUIList.AddPlayerToCarousel(connectedPlayer);
        }

        if (HostData.GetGame().GetGameName() == "War")
        {
            warPanel.SetActive(true);
            standardPanel.SetActive(false);
            goFishPanel.SetActive(false);
            shuffleButton.gameObject.SetActive(false);
        }
        else if (HostData.GetGame().GetGameName().Equals("GoFish"))
        {
            warPanel.SetActive(false);
            standardPanel.SetActive(false);
            goFishPanel.SetActive(true);

            goFishDeckCardsUI[0].SetActive(true);
            goFishDeckCardsUI[1].SetActive(true);
            goFishDeckCardsUI[2].SetActive(true);
            shuffleButton.gameObject.SetActive(false);
        }
        else
        {
            warPanel.SetActive(false);
            standardPanel.SetActive(true);
            goFishPanel.SetActive(false);
            shuffleButton.gameObject.SetActive(true);
        }

        chatOptions.RefreshShownValue();
        chatPlace.offsetMin = new Vector2(chatPlace.offsetMin.x, 150);
        chatPlace.offsetMax = new Vector2(chatPlace.offsetMax.x, 215);
        
        if (!HostData.isFreeplay())
        {
            DeclareWinnerButton.gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HostData.GetGame().IsGameEmpty() && isGameEnded)
        {
            // HostData.GetGame().ClearAll();
            HostData.resetGame();
            SceneManager.LoadScene(SceneNames.WaitingRoomScreen);
        }

        try
        {
            currentPlayer.GetComponent<Text>().text = HostData.GetGame().GetPlayerOfCurrentTurn().username + "'s Turn";
        }
        catch(Exception ex)
        {
            Debug.LogError("GameScreenController.cs error: " + ex.Message);
        }

        if (hasInitializedGame == false && startTime + secondsBeforeInitialization <= Time.time)
        {
            hasInitializedGame = true;
            HostData.GetGame().InitializeGame();
        }

        DisplayCards();
        // updatingChat(); If chatOptions aren't updating, reenable this.

        // if the notification window should be shown
        string[] messages = new string[] { };
        if (HostData.GetDoShowNotificationWindow(ref messages))
        {
            for (int x = 0; x < messages.Length; x++)
            {
                notificationWindow.ShowNotification(messages[x]);
            }

            HostData.SetDoShowNotificationWindow(false);
        }

        if (isDeclaringWinner)
        {
            winnerPanel.SetActive(false);
            // Display winner message
            gameOverPanel.SetActive(true);
            gameOverText.GetComponent<Text>().text =
                "Congratulations, " + winnerDropdown.options[winnerDropdown.value].text + "!";
            isDeclaringWinner = false;
        }

        if (Game.didSkipTurn)
        {
            timer.StartTimer();
            Game.didSkipTurn = false;
        }

        if (HostData.DidLastPlayedCardTextureUpdate())
        {
            if (HostData.GetGame().GetGameName().Equals("Freeplay"))
            {
                if (HostData.getDisplayLastCard())
                {
                    lastPlayedCard.texture = HostData.GetLastPlayedCardTexture();
                }
            }
            else
            {
                lastPlayedCard.texture = HostData.GetLastPlayedCardTexture();
            }
        }

        lastPlayedDeckOne.texture = textureOne;
        lastPlayedDeckTwo.texture = textureTwo;

        if (goFishPanel.activeInHierarchy)
        {
            int cardCount = HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCardCount();
            if (cardCount > 3)
            {
                goFishDeckCardsUI[0].SetActive(true);
                goFishDeckCardsUI[1].SetActive(true);
                goFishDeckCardsUI[2].SetActive(true);
            }
            else if (cardCount == 2)
            {
                goFishDeckCardsUI[0].SetActive(true);
                goFishDeckCardsUI[1].SetActive(true);
                goFishDeckCardsUI[2].SetActive(false);
            }
            else if (cardCount == 1)
            {
                goFishDeckCardsUI[0].SetActive(true);
                goFishDeckCardsUI[1].SetActive(false);
                goFishDeckCardsUI[2].SetActive(false);
            }
            else if (cardCount == 0)
            {
                goFishDeckCardsUI[0].SetActive(false);
                goFishDeckCardsUI[1].SetActive(false);
                goFishDeckCardsUI[2].SetActive(false);
            }
        }

        if (doFlipWarCards)
        {
            StartCoroutine(DelayCards());
            doFlipWarCards = false;
        }

    }
    
    private IEnumerator DelayCards()
    {
        yield return new WaitForSeconds(3);

        GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
        GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + "SingleCardBack");

        if (HostData.GetGame().GetDeck(DeckChoices.PONEUNPLAYED).GetCardCount() == 52)
        {
        // declare a winner with raising an event
        object[] content = new object[] { "Player one" };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);

        }
        else if (HostData.GetGame().GetDeck(DeckChoices.PTWOUNPLAYED).GetCardCount() == 52)
        {
        // declare a winner with raising an event
        object[] content = new object[] { "Player two" };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);
        }

    }

    public void updatingChat() {
        int chatValue = chatOptions.value;
        // chatOptions.RefreshShownValue();
        if (chatValue == 0) // normal chat
        {
            chatPlace.offsetMin = new Vector2(chatPlace.offsetMin.x, 150);
            chatPlace.offsetMax = new Vector2(chatPlace.offsetMax.x, 215);
            HostData.setChatAllowed(true);
            // chatPanel.SetActive(true);
            chatCanvas.GetComponent<CanvasGroup>().alpha = 1;
            MessageContent.SetActive(true);
        }
        else if (chatValue == 1) // disable chat
        {
            chatPlace.offsetMin = new Vector2(chatPlace.offsetMin.x, 150);
            chatPlace.offsetMax = new Vector2(chatPlace.offsetMax.x, 215);
            HostData.setChatAllowed(false);
            // chatPanel.SetActive(false);
            chatCanvas.GetComponent<CanvasGroup>().alpha = 0;
            MessageContent.SetActive(false);
        }
        else // mute chat
        {
            chatPlace.offsetMin = new Vector2(chatPlace.offsetMin.x, 150);
            chatPlace.offsetMax = new Vector2(chatPlace.offsetMax.x, 215);
            HostData.setChatAllowed(true);
            // chatPanel.SetActive(false);
            chatCanvas.GetComponent<CanvasGroup>().alpha = 0;
            MessageContent.SetActive(false);
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(HostData.ToHashtable());
    }

    public void DisplayCards()
    {
        for (int i = 0; i < HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetCardCount(); ++i)
        {
            if (!playedCardMenu.FindCardFromCarousel(HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetCard(i)))
            {
                HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetCard(i).Print();
                Debug.Log(HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetCardCount());
                StandardCard card = (StandardCard) HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetCard(i);
                Debug.Log(card.GetRank());
                Debug.Log(card.GetSuit());
                playedCardMenu.AddCardToCarousel(card, CardTypes.StandardCard);

                // remove it from undealt card menu if it was present
                if (undealtCardMenu.FindCardFromCarousel(card))
                {
                    undealtCardMenu.RemoveCardFromCarousel(card);
                }
            }
        }

        for (int i = 0; i < HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCardCount(); ++i)
        {
            if (!undealtCardMenu.FindCardFromCarousel(HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCard(i)))
            {
                HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCard(i).Print();
                Debug.Log(HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCardCount());
                StandardCard card = (StandardCard) HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCard(i);
                Debug.Log(card.GetRank());
                Debug.Log(card.GetSuit());
                undealtCardMenu.AddCardToCarousel(card, CardTypes.StandardCard);

                // remove it from played card menu if it was present
                if (playedCardMenu.FindCardFromCarousel(card))
                {
                    playedCardMenu.RemoveCardFromCarousel(card);
                }
            }
        }

        List<Card> cardsInCarousel = undealtCardMenu.GetAllCardsInCarousel();
        int cardCount = cardsInCarousel.Count;
        for (int i = cardCount - 1; i >= 0; i--)
        {
            // if the card carousel contains a card that is not present in the deck, remove it from carousel
            if (!HostData.GetGame().GetDeck(DeckChoices.UNDEALT).IsCardPresent(cardsInCarousel[i]))
            {
                undealtCardMenu.RemoveCardFromCarousel(cardsInCarousel[i]);
            }
        }
    }

    /// <summary>
    /// This method verifies if a card is allowed to be played based on the selected game's rules
    /// </summary>
    /// <param name="card">The card to see if it is valid to play</param>
    /// <returns></returns>
    public bool VerifyIfCardIsValid(Card card)
    {
        return HostData.GetGame().VerifyMove(card);
    }

    /// <summary>
    /// This method returns a boolean depending on whether or not a player is allowed to skip their turn
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public bool VerifyCanSkipTurn(int playerIndex)
    {
        return HostData.GetGame().VerifyCanSkip(playerIndex);
    }

    /// <summary>
    /// This sends a signal to all the clients to either enable or disable the timer
    /// </summary>
    public void EnableTimer(bool enable)
    {
        timer.EnableTimer(enable);
        if (HostData.GetTimerSeconds() + HostData.GetTimerMinutes() > 0)
        {
            PhotonScripts.NetworkController.EnableTimer(enable);
        }
        else
        {
            Debug.Log("Must have enabled timer in pregame settings to toggle it");
        }
    }

    /// <summary>
    /// Shows or hides the settingsWindow depending on <paramref name="enabled"/>
    /// </summary>
    /// <param name="enabled"></param>
    public void DisplaySettingsWindow(bool enabled)
    {
        settingsPanel.SetActive(enabled);
    }

    // Adding functions for endgame button and declare winner button

    public void EndGameClicked()
    {
        endGamePanel.SetActive(true);
    }

    public void KeepPlayingClicked()
    {
        endGamePanel.SetActive(false);
    }

    public void DeclareWinnerClicked()
    {
        //HostData.clearGame();

        winnerPanel.SetActive(true);
        var allConnectedPlayers = HostData.GetGame().GetAllPlayers();
        foreach (PlayerInfo player in allConnectedPlayers)
        {
            Debug.Log(player.photonPlayer.NickName);
            winnerDropdown.options.Add(new Dropdown.OptionData(player.photonPlayer.NickName));
        }
    }

    public void ExitClicked()
    {
        winnerPanel.SetActive(false);
    }

    /// <summary>
    /// This method ends the game and declares a method.
    /// </summary>
    /// <param name="username">The PhotonNetwork.NickName of the user who won the game</param>
    /// <param name="winningMessage">The message to be displayed</param>
    public static void DeclareWinner(string username, string winningMessage)
    {
        Debug.Log(winningMessage);

        object[] content = new object[] {username};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);

        isDeclaringWinner = true;
        isGameEnded = false;
    }

    public void DeclareWinnerChoiceClicked()
    {
        // this will raise an event

        DeclareWinner(winnerDropdown.options[winnerDropdown.value].text,
            "Winner Declared! Congratulations, " + winnerDropdown.options[winnerDropdown.value].text);
    }

    public void ExitGameClicked()
    {
        Debug.Log("exit game clicked");
        HostData.clearGame();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(SceneNames.LandingPage);
    }

    public void GoToGameOverFromEndGame()
    {
        // HostData.clearGame();
        endGamePanel.SetActive(false);

        object[] content = new object[] {"nowinner"};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);

        isGameEnded = false;
        gameOverText.GetComponent<Text>().text = "Game is over.";
        gameOverPanel.SetActive(true);
    }

    public void TimerEarlyWarning()
    {
        // do nothing for now
    }

    public void TimerReachedZero()
    {
        // do nothing right now
    }
}