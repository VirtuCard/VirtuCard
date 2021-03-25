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
    public Text currentPlayer;

    public GameObject settingsPanel;

    public Dropdown chatOptions;
    public Toggle timerToggle;

    public GameObject winnerPanel;
    public Dropdown winnerDropdown;
    public GameObject gameOverPanel;
    public GameObject gameOverText;
    public GameObject endGamePanel;


    public GameObject playedCardCarousel;
    public GameObject undealtCardCarousel;
    private CardMenu playedCardMenu;
    private CardMenu undealtCardMenu;

    private bool hasInitializedGame = false;

    private float startTime;

    // this is the time in seconds before the game intitializes
    private int secondsBeforeInitialization = 2;


    // Start is called before the first frame update
    void Start()
    {
        if (HostData.isChatAllowed())
        {
            allOfChatUI.SetActive(true);
        }
        else
        {
            allOfChatUI.SetActive(false);
        }

        startTime = Time.time;
        playedCardMenu = playedCardCarousel.GetComponent<CardMenu>();
        undealtCardMenu = undealtCardCarousel.GetComponent<CardMenu>();

        // setup settings menu
        settingsPanel.SetActive(false);
        timerToggle.SetIsOnWithoutNotify(HostData.IsTimerEnabled());
        timerToggle.onValueChanged.AddListener(delegate { EnableTimer(timerToggle.isOn); });
        timerToggle.gameObject.SetActive(HostData.IsTimerEnabled());


    }

    // Update is called once per frame
    void Update()
    {
        if (HostData.GetGame().IsGameEmpty())
        {
            SceneManager.LoadScene(SceneNames.WaitingRoomScreen);
        }

        currentPlayer.GetComponent<Text>().text = HostData.GetGame().GetPlayerOfCurrentTurn().username + "'s Turn";
        if (hasInitializedGame == false && startTime + secondsBeforeInitialization <= Time.time)
        {
            hasInitializedGame = true;
            HostData.GetGame().InitializeGame();
        }

        DisplayCards();
        updatingChat();

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
    }
    
    public void updatingChat() {
        int chatValue = chatOptions.value;
        if (chatValue == 0) // normal chat
        {
            HostData.setChatAllowed(true);
            chatPanel.SetActive(true);
        }
        else if (chatValue == 1) // disable chat
        {
            HostData.setChatAllowed(false);
            chatPanel.SetActive(false);
        }
        else // mute chat
        {
            HostData.setChatAllowed(true);
            chatPanel.SetActive(false);
        }
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
        winnerPanel.SetActive(true);
        var allConnectedPlayers = HostData.GetGame().GetAllPlayers();
        foreach (PlayerInfo player in allConnectedPlayers) {
            winnerDropdown.options.Add(new Dropdown.OptionData(player.photonPlayer.NickName));
        }
    
    }

    public void ExitClicked()
    {
        winnerPanel.SetActive(false);
    
    }

    public void DeclareWinnerChoiceClicked()
    {
        // this will raise an event
        Debug.Log("Winner Declared! Congratulations, " +  winnerDropdown.options[winnerDropdown.value].text);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        object[] content = new object[] {winnerDropdown.options[winnerDropdown.value].text};
        PhotonNetwork.RaiseEvent(20, content, raiseEventOptions, SendOptions.SendUnreliable);
        winnerPanel.SetActive(false);
        // Display winner message
        gameOverPanel.SetActive(true);
        gameOverText.GetComponent<Text>().text = "Congratulations, " + winnerDropdown.options[winnerDropdown.value].text + "!";

    }

    public void ExitGameClicked()
    {
        Debug.Log("exit game clicked");
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(SceneNames.LandingPage, LoadSceneMode.Single);
    }
}