using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameScreenController : MonoBehaviour
{
    public GameObject allOfChatUI;
    public GameObject chatPanel;
    public Toggle chatToggle;
    public Text currentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        //HostData.setChatAllowed(false);
        //Debug.Log("CHATCHAT CHATTTTTT is " + HostData.isChatAllowed());
        if (HostData.isChatAllowed())
        {
            allOfChatUI.SetActive(true);
            chatToggle.SetIsOnWithoutNotify(HostData.isChatAllowed());
            chatToggle.onValueChanged.AddListener(delegate { ChatToggleValueChanged(chatToggle.isOn); });
        }
        else {
            allOfChatUI.SetActive(false);
        }

        HostData.GetGame().AdvanceTurn(true);
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayer.GetComponent<Text>().text = HostData.GetGame().GetPlayerOfCurrentTurn().username + "'s Turn";
    }
    
    /// <summary>
    /// This method should be called when a client has requested to draw a single card
    /// It draws a random card from the undealt deck and then removes it.
    /// </summary>
    /// <returns></returns>
    public Card DrawCard()
    {
        return HostData.GetGame().DrawCardFromDeck(DeckChoices.UNDEALT);
    }

    /// <summary>
    /// This method should be called when a client has requested to draw multiple cards.
    /// It draws <paramref name="numOfCards"/> random cards from the undealt deck and subsequently removes them.
    /// </summary>
    /// <param name="numOfCards">Number of cards to draw and return</param>
    /// <returns>A list containing all the drawn cards</returns>
    public List<Card> DrawCards(int numOfCards)
    {
        return HostData.GetGame().DrawCardsFromDeck(numOfCards, DeckChoices.UNDEALT);
    }

    /// <summary>
    /// TODO implementation
    /// </summary>
    /// <param name="card">The card to play</param>
    /// <param name="playerIndex">The index of the player playing the card</param>
    /// <returns>Returns true or false depending on whether the card was played</returns>
    public bool PlayCard(Card card, int playerIndex)
    {
        if (VerifyIfCardIsValid(card))
        {
            HostData.GetGame().AddCardToDeck(card, DeckChoices.PLAYED);
            return true;
        }
        return false;
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
    /// This method is called when the chat toggle state changes
    /// </summary>
    /// <param name="toggleVal"></param>
    private void ChatToggleValueChanged(bool toggleVal)
    {
        // HostData.setChatAllowed(toggleVal);
        // Debug.Log("Chat is " + HostData.isChatAllowed());
        chatPanel.SetActive(!toggleVal);
    }
}
