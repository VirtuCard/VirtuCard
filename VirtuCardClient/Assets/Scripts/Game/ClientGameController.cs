using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientGameController : MonoBehaviour
{
    public Button skipBtn;
    public Button playCardBtn;

    private CardDeck cards = new CardDeck();

    public GameObject cardCarousel;
    private CardMenu cardMenu;
    public GameObject chatPanel;
    public Toggle chatToggle;


    private bool wasCurrentlyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        skipBtn.onClick.AddListener(delegate() {
            SkipBtnClicked();
        });
        playCardBtn.onClick.AddListener(delegate () {
            PlayCardBtnClicked();
        });
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
    }

    private void PlayCardBtnClicked()
    {
        StandardCard card = (StandardCard)cardMenu.GetCurrentlySelectedCard();
        card.Print();
        RemoveCard(card);
    }

    /// <summary>
    /// This method is called when the chat toggle state changes
    /// </summary>
    /// <param name="toggleVal"></param>
    private void ChatToggleValueChanged(bool toggleVal)
    {
        chatPanel.SetActive(!toggleVal);
    }
}
