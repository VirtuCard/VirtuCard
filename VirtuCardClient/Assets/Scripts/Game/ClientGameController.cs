using ExitGames.Client.Photon;
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
    
    private CardDeck cards = new CardDeck();

    public GameObject cardCarousel;
    private CardMenu cardMenu;
    public GameObject chatDisableSign;

    public GameObject chatPanel;
    public Toggle chatToggle;
    public GameObject chatDisable;


    // this is used to determine if the user has scrolled over to a new card, so it can be used to verify
    private Card previouslySelectedCard;
    bool cardIsValid = false;
    public Text cardIsValidText;

    private bool wasCurrentlyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        
        ClientData.setChatAllowed(false);
        if (!ClientData.isChatAllowed())
        {
            Debug.Log("Chat Not Allowed.");
            chatDisable.SetActive(false);
            chatDisableSign.SetActive(true);
        }
        else { chatDisableSign.SetActive(false); }

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
            wasCurrentlyTurn = false;
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
        // send request for a new card
        int numOfCards = 1;
        object[] content = new object[] { PhotonNetwork.NickName, numOfCards };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(7, content, raiseEventOptions, SendOptions.SendUnreliable);
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
    /// It should setup all the necessary things like enabling skip btn, etc...
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
        }
        else {
            errorDisplay.GetComponent<Text>().text = "You are not allowed to skip!";
        }
    }

    private void PlayCardBtnClicked()
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

    /// <summary>
    /// This method is called when the chat toggle state changes
    /// </summary>
    /// <param name="toggleVal"></param>
    private void ChatToggleValueChanged(bool toggleVal)
    {
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
            if (currentPersonsTurn.Equals(PhotonNetwork.NickName))
            {
                ClientData.setCurrentTurn(true);
            }
            else
            {
                ClientData.setCurrentTurn(false);
            }
        }
    }

    private void SendCardToHost(Card card)
    {
        if (card.GetType().Name == "StandardCard")
        {
            Debug.Log("Sending Card: " + card.ToString());
            StandardCard cardToSend = (StandardCard)card;
            object[] content = new object[] { "StandardCard", cardToSend.GetRank(), cardToSend.GetSuit() };
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
}
