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
using System.IO;
using System.Threading.Tasks;
using GameScreen.GameLogic.Cards;
using SFB;
using System.Linq;

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
    public Toggle censorChatToggle;

    public GameObject winnerPanel;
    public Dropdown winnerDropdown;
    public GameObject gameOverPanel;
    public GameObject gameOverText;
    public GameObject endGamePanel;

    public GameObject kickPlayerPanel;
    public GameObject playerKickedPanel;
    public Dropdown kickPlayerDropdown;
    public Text playerKickedName;

    public GameObject warPanel;
    public GameObject goFishPanel;
    public GameObject standardPanel;
    public GameObject pokerPanel;

    [Header("Poker")]
    public Text PokerPotText;
    public Text PokerBetText;

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

    private bool doPlayedAnimation = false;
    bool hasKickPlayersBeenInitialized = true;


    [Header("Background Changing")] public Button setBackgroundBtn;
    public Button defBackgroundBtn;
    public RawImage mainCanvasImage;
    string filePath;
    public bool setBackground;

    [Header("Sleeve Changing")] public Button setSleeveBtn;
    public Button defSleeveBtn;
    string sleeveFilePath;
    public bool setSleeve;
    private Texture2D backTex;
    public RawImage undersideUI;
    public RawImage cardDeck;
    public RawImage cardDeck1;
    public RawImage cardDeck2;
    public Texture defBackTex;
    public Text sizeWarningText;

    [Header("GoFish Cards")] public RawImage cardDeckGoFish;
    public RawImage cardDeckGoFish1;
    public RawImage cardDeckGoFish2;

    [Header("War Cards")] public RawImage unplayedDeck1;
    public RawImage unplayedDeck2;
    public RawImage playedDeck1;
    public RawImage playedDeck2;

    [Header("Freeplay Stuff")] public Button flipCardBtn;


    // Start is called before the first frame update
    void Start()
    {
        isDeclaringWinner = false;
        isGameEnded = true;
        setBackground = false;
        defBackgroundBtn.interactable = false;
        setSleeve = false;
        defBackTex = null;
        defSleeveBtn.interactable = false;
        if (HostData.isChatAllowed())
        {
            allOfChatUI.SetActive(true);

            /*
            // check if chat is muted from the waiting setting screen
            if (HostData.isChatMute())
            {
                chatOptions.value = 2;
                updatingChat();
            }*/
        }
        else
        {
            allOfChatUI.SetActive(false);
        }

        hasKickPlayersBeenInitialized = true;
        startTime = Time.time;
        playedCardMenu = playedCardCarousel.GetComponent<CardMenu>();
        undealtCardMenu = undealtCardCarousel.GetComponent<CardMenu>();

        // setup settings menu
        chatOptions.onValueChanged.AddListener(e => updatingChat());
        settingsPanel.SetActive(false);
        timerToggle.SetIsOnWithoutNotify(HostData.IsTimerEnabled());
        timerToggle.onValueChanged.AddListener(delegate { EnableTimer(timerToggle.isOn); });
        timerToggle.gameObject.SetActive(HostData.IsTimerEnabled());

        censorChatToggle.SetIsOnWithoutNotify(HostData.isChatCensored());
        censorChatToggle.onValueChanged.AddListener(delegate { EnableProfanity(censorChatToggle.isOn); });

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
            pokerPanel.SetActive(false);
            shuffleButton.gameObject.SetActive(false);
            flipCardBtn.gameObject.SetActive(false);
        }
        else if (HostData.GetGame().GetGameName().Equals("GoFish"))
        {
            warPanel.SetActive(false);
            standardPanel.SetActive(false);
            goFishPanel.SetActive(true);
            pokerPanel.SetActive(false);

            goFishDeckCardsUI[0].SetActive(true);
            goFishDeckCardsUI[1].SetActive(true);
            goFishDeckCardsUI[2].SetActive(true);
            shuffleButton.gameObject.SetActive(false);
            flipCardBtn.gameObject.SetActive(false);
        }
        else if (HostData.GetGame().GetGameName().Equals("Poker"))
        {
            warPanel.SetActive(false);
            standardPanel.SetActive(false);
            goFishPanel.SetActive(false);
            pokerPanel.SetActive(true);
            shuffleButton.gameObject.SetActive(false);
            flipCardBtn.gameObject.SetActive(false);
        }
        else
        {
            warPanel.SetActive(false);
            standardPanel.SetActive(true);
            goFishPanel.SetActive(false);
            pokerPanel.SetActive(false);
            shuffleButton.gameObject.SetActive(true);
            if (HostData.GetGame().GetGameName().Equals("Freeplay"))
            {
                flipCardBtn.gameObject.SetActive(false);
                flipCardBtn.onClick.AddListener(delegate { freeplayFlipCardBtnClicked(); });
                if (HostData.getDisplayLastCard())
                {
                    flipCardBtn.GetComponentInChildren<Text>().text = "Hide Card";
                }
                else
                {
                    flipCardBtn.GetComponentInChildren<Text>().text = "Flip Card";
                }
            }
            else
            {
                flipCardBtn.gameObject.SetActive(false);
            }
        }

        chatOptions.RefreshShownValue();
        chatPlace.offsetMin = new Vector2(chatPlace.offsetMin.x, 150);
        chatPlace.offsetMax = new Vector2(chatPlace.offsetMax.x, 215);

        if (!HostData.isFreeplay())
        {
            DeclareWinnerButton.gameObject.SetActive(false);
        }

        // Initialize the kick player dropdown
        /*
        var allConnectedPlayers = HostData.GetGame().GetAllPlayers();
        foreach (PlayerInfo player in allConnectedPlayers)
        {
            //Debug.Log(player.photonPlayer.NickName);
            kickPlayerDropdown.options.Add(new Dropdown.OptionData(player.photonPlayer.NickName));
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (defBackTex == null)
        {
            defBackTex = Resources.Load<Texture>("Card UI/SingleCardBack");
        }

        if (HostData.GetGame().IsGameEmpty() && isGameEnded)
        {
            // HostData.GetGame().ClearAll();
            HostData.resetGame();
            SceneManager.LoadScene(SceneNames.WaitingRoomScreen);
        }

        try
        {
            if (HostData.GetGame().GetGameName().Equals("Poker"))
            {
                PokerBetText.text = "Bet To Match: " + ((Poker)HostData.GetGame()).GetCurrentBet().ToString();
                PokerPotText.text = "Current Pot: " + ((Poker)HostData.GetGame()).GetCurrentPot().ToString();
            }
        }
        catch { /* just an empty catch here */ };

        try
        {
            currentPlayer.GetComponent<Text>().text = HostData.GetGame().GetPlayerOfCurrentTurn().username + "'s Turn";
        }
        catch (Exception ex)
        {
            Debug.LogError("GameScreenController.cs error: " + ex.Message);
        }

        if (hasInitializedGame == false && startTime + secondsBeforeInitialization <= Time.time)
        {
            hasInitializedGame = true;
            HostData.GetGame().InitializeGame();
        }

//        DisplayCards();
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

        if (doPlayedAnimation)
        {
            lastPlayedCard.gameObject.SetActive(true);
            doPlayedAnimation = false;
            if (HostData.GetGame().GetGameName().Equals("Freeplay"))
            {
                // enable this button
                flipCardBtn.gameObject.SetActive(true);

                if (HostData.getDisplayLastCard())
                {
                    lastPlayedCard.texture = HostData.GetLastPlayedCardTexture();
                }
                else
                {
                    if (setSleeve)
                    {
                        lastPlayedCard.texture = backTex;
                    }
                    else
                    {
                        lastPlayedCard.texture = Resources.Load<Texture>("Card UI/SingleCardBack");
                    }

                    // call this just to reset the texture boolean
                    HostData.GetLastPlayedCardTexture();
                }
            }
            else
            {
                lastPlayedCard.texture = HostData.GetLastPlayedCardTexture();
            }
        }

        if (HostData.DidLastPlayedCardTextureUpdate())
        {
            doPlayedAnimation = true;
            lastPlayedCard.gameObject.SetActive(false);
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

        //This is in for testing purposes
        //playerUIList.UpdateUI();

        if (filePath != null)
        {
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

                Debug.Log(filePath);
                Debug.Log(tex);
                mainCanvasImage.color = Color.white;
                mainCanvasImage.texture = tex;
                setBackground = true;
                defBackgroundBtn.interactable = true;
            }

            filePath = null;
        }

        if (sleeveFilePath != null)
        {
            if (sleeveFilePath.Length != 0)
            {
                var fileInfo = new FileInfo(sleeveFilePath);
                Debug.Log(fileInfo.Length);
                if (fileInfo.Length > 510000)
                {
                    sizeWarningText.gameObject.SetActive(true);
                    return;
                }
                else
                {
                    sizeWarningText.gameObject.SetActive(false);
                }
            }

            if (sleeveFilePath.Length != 0)
            {
                backTex = null;
                byte[] fileData;
                if (File.Exists(sleeveFilePath))
                {
                    fileData = File.ReadAllBytes(sleeveFilePath);
                    object[] content = new object[] {fileData};
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
                    PhotonNetwork.RaiseEvent((int) NetworkEventCodes.SleeveChanged, content, raiseEventOptions,
                        SendOptions.SendUnreliable);
                    backTex = new Texture2D(2, 2);
                    backTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                }

                Debug.Log(filePath);
                Debug.Log(backTex);
                if (lastPlayedCard.texture != HostData.GetLastPlayedCardTexture())
                {
                    lastPlayedCard.texture = backTex;
                }

                undersideUI.texture = backTex;
                cardDeck.texture = backTex;
                cardDeck1.texture = backTex;
                cardDeck2.texture = backTex;

                cardDeckGoFish.texture = backTex;
                cardDeckGoFish1.texture = backTex;
                cardDeckGoFish2.texture = backTex;

                unplayedDeck1.texture = backTex;
                unplayedDeck2.texture = backTex;
                playedDeck1.texture = backTex;
                playedDeck2.texture = backTex;

                setSleeve = true;
                defSleeveBtn.interactable = true;
            }

            sleeveFilePath = null;
        }
    }

    private void EnableProfanity(bool censorProfanity)
    {
        HostData.setChatCensored(censorProfanity);

        object[] content = new object[] {censorProfanity};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.UpdateProfanity, content, raiseEventOptions,
            SendOptions.SendUnreliable);
    }

    private void freeplayFlipCardBtnClicked()
    {
        if (HostData.getDisplayLastCard())
        {
            flipCardBtn.GetComponentInChildren<Text>().text = "Flip Card";
        }
        else
        {
            flipCardBtn.GetComponentInChildren<Text>().text = "Hide Card";
        }

        HostData.setDisplayLastCard(!HostData.getDisplayLastCard());
        doPlayedAnimation = true;
    }

    private IEnumerator DelayCards()
    {
        yield return new WaitForSeconds(3);

        if (setSleeve)
        {
            GameScreenController.textureOne = backTex;
            GameScreenController.textureTwo = backTex;
        }
        else
        {
            GameScreenController.textureOne = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
            GameScreenController.textureTwo = Resources.Load<Texture>("Card UI/" + "SingleCardBack");
        }

        if (HostData.GetGame().GetDeck(DeckChoices.PONEUNPLAYED).GetCardCount() == 52)
        {
            // declare a winner with raising an event
            object[] content = new object[] {"Player one"};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int) NetworkEventCodes.WinnerSelected, content, raiseEventOptions,
                SendOptions.SendUnreliable);
        }
        else if (HostData.GetGame().GetDeck(DeckChoices.PTWOUNPLAYED).GetCardCount() == 52)
        {
            // declare a winner with raising an event
            object[] content = new object[] {"Player two"};
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent((int) NetworkEventCodes.WinnerSelected, content, raiseEventOptions,
                SendOptions.SendUnreliable);
        }
    }

    public void updatingChat()
    {
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
                Card card = HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetCard(i);
                if (card.GetType() == typeof(UnoCard))
                {
                    playedCardMenu.AddCardToCarousel(card, CardTypes.UnoCard);
                }
                else
                {
                    playedCardMenu.AddCardToCarousel(card, CardTypes.StandardCard);
                }

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
                Card card = HostData.GetGame().GetDeck(DeckChoices.UNDEALT).GetCard(i);
                if (card.GetType() == typeof(UnoCard))
                {
                    playedCardMenu.AddCardToCarousel(card, CardTypes.UnoCard);
                }
                else
                {
                    playedCardMenu.AddCardToCarousel(card, CardTypes.StandardCard);
                }

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
        sizeWarningText.gameObject.SetActive(false);
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

    public void KickPlayerClicked()
    {
        kickPlayerPanel.SetActive(true);

        // Initialize the kick player dropdown
        if (hasKickPlayersBeenInitialized){
          var allConnectedPlayers = HostData.GetGame().GetAllPlayers();
          foreach (PlayerInfo player in allConnectedPlayers)
          {
            //Debug.Log(player.photonPlayer.NickName);
              kickPlayerDropdown.options.Add(new Dropdown.OptionData(player.photonPlayer.NickName));
          }
          hasKickPlayersBeenInitialized = false;
        }
        // still have to remove players that have been kicked
    }

    public void OnKickPlayerChosen()
    {
        string toKick = kickPlayerDropdown.options[kickPlayerDropdown.value].text;

        // Update dropdown
        kickPlayerDropdown.options.RemoveAt(kickPlayerDropdown.value);
        kickPlayerDropdown.value = kickPlayerDropdown.value == 0 ? 1 : 0;

        object[] content = new object[] {toKick};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.PlayerKicked, content, raiseEventOptions,
            SendOptions.SendUnreliable);

        kickPlayerPanel.SetActive(false);
        playerKickedName.GetComponent<Text>().text = toKick + " was removed from the game.";
        playerKickedPanel.SetActive(true);
    }

    public void ExitDisplayPlayerKickedPanel()
    {
        playerKickedPanel.SetActive(false);
    }

    public void ExitKickPlayerClicked()
    {
        kickPlayerPanel.SetActive(false);
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
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.WinnerSelected, content, raiseEventOptions,
            SendOptions.SendUnreliable);

        isDeclaringWinner = true;
        isGameEnded = false;
    }

    /// <summary>
    /// This method is called when the Change background button is clicked
    /// </summary>
    public void UploadButtonClicked()
    {
        Task t = Task.Run(UploadButtonAsync);
//         filePath = EditorUtility.OpenFilePanel("Select your custom background", "", "png,jpg,jpeg,");
    }

    void UploadButtonAsync()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
        };
        var files = StandaloneFileBrowser.OpenFilePanel("Select your custom background", "", extensions, false);
        if (files.Length > 0)
        {
            filePath = files[0];
        }
        else
        {
            filePath = "";
        }
    }

    /// <summary>
    /// This method is called when the Default background button is clicked
    /// </summary>
    public void DefButtonClicked()
    {
        Debug.Log(mainCanvasImage.texture);
        mainCanvasImage.texture = null;
        mainCanvasImage.color = new Color(0.03921569f, 0.4235294f, 0.01176471f, 1);
        defBackgroundBtn.interactable = false;
        setBackground = false;
    }

    public void UploadSleeveButtonClicked()
    {
//         filePath = EditorUtility.OpenFilePanel("Select your custom background", "", "png,jpg,jpeg,");

        Task.Run(UploadButtonAsync);
    }

    void UploadSleeveAsync()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
        };
        var files = StandaloneFileBrowser.OpenFilePanel("Select your custom sleeve", "", extensions, false);
        if (files.Length > 0)
        {
            sleeveFilePath = files[0];
        }
        else
        {
            sleeveFilePath = "";
        }
    }

    /// <summary>
    /// This method is called when the Default sleeve button is clicked
    /// </summary>
    public void DefSleeveButtonClicked()
    {
        Debug.Log("Default Button Clicked");
        StartCoroutine(SetDefCardBack());
        setSleeve = true;
        defSleeveBtn.interactable = false;
    }

    private IEnumerator SetDefCardBack()
    {
        yield return new WaitForSeconds(1);
        if (lastPlayedCard.texture != HostData.GetLastPlayedCardTexture())
        {
            lastPlayedCard.texture = defBackTex;
        }

        undersideUI.texture = defBackTex;
        cardDeck.texture = defBackTex;
        cardDeck1.texture = defBackTex;
        cardDeck2.texture = defBackTex;

        cardDeckGoFish.texture = defBackTex;
        cardDeckGoFish1.texture = defBackTex;
        cardDeckGoFish2.texture = defBackTex;

        unplayedDeck1.texture = defBackTex;
        unplayedDeck2.texture = defBackTex;
        playedDeck1.texture = defBackTex;
        playedDeck2.texture = defBackTex;
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

        var players = HostData.GetGame().GetAllPlayers();
        foreach (var player in players)
        {
            playerUIList.RemovePlayerFromCarousel(player.username);
        }
        
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
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.WinnerSelected, content, raiseEventOptions,
            SendOptions.SendUnreliable);

        isGameEnded = false;
        gameOverText.GetComponent<Text>().text = "Game is over.";
        gameOverPanel.SetActive(true);
    }

    public void OnPlayAgainClicked()
    {
        gameOverPanel.SetActive(false);

        object[] content = new object[] {"playagain"};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent((int) NetworkEventCodes.PlayAgain, content, raiseEventOptions,
            SendOptions.SendUnreliable);

        //TODO reset:
        // Card deck backend
        // Card deck UI
        // Card Player Carousel

        List<PlayerInfo> players = HostData.GetGame().GetAllPlayers();
        //currentPot = players.Count * ANTE;
        foreach (PlayerInfo player in players)
        {
            player.score = 0;
            player.cards.RemoveAllCards();
        }

        // This should reset the card deck backend
        playerUIList.UpdateUI();
        HostData.GetGame().ResetAllDecks();
        HostData.GetGame().InitializeGame();

        // Check carousel and redo it
        // This is probably what needs to be used PlayerList.UpdateUI(); - ask Kade

        // Reset Card Deck UI
        string gameType = (String) HostData.GetGame().GetGameName();
        if (gameType == "War")
        {
/*          textureOne = Resources.Load<Texture>("Card UI/SingleCardBack");
            textureTwo = Resources.Load<Texture>("Card UI/SingleCardBack");*/
            if (setSleeve)
            {
                textureOne = backTex;
                textureTwo = backTex;
            }
            else
            {
                textureOne = defBackTex;
                textureTwo = defBackTex;
            }
        }
        else if (gameType == "GoFish")
        {
            //This one probably needs help - ask Kade
            defBackTex = Resources.Load<Texture>("Card UI/SingleCardBack");
            HostData.SetLastPlayedCardTexture("SingleCardBack");
        }
        else if (gameType == "TestGame")
        {
            //defBackTex = Resources.Load<Texture>("Card UI/SingleCardBack");

            HostData.SetLastPlayedCardTexture("SingleCardBack");
        }

        // else if (gameType == "NewGameHere)
        // reset cardbacks
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