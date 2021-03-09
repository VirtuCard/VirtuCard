using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameScreenController : MonoBehaviour
{
    public GameObject chatPanel;
    public Toggle chatToggle;

    // the game being played
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        chatToggle.SetIsOnWithoutNotify(HostData.isChatAllowed());
        chatToggle.onValueChanged.AddListener(delegate { ChatToggleValueChanged(chatToggle.isOn); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This method should be called when a client has requested to draw a single card
    /// It draws a random card from the undealt deck and then removes it.
    /// </summary>
    /// <returns></returns>
    public Card DrawCard()
    {
        return game.DrawCardFromDeck(DeckChoices.UNDEALT);
    }

    /// <summary>
    /// This method should be called when a client has requested to draw multiple cards.
    /// It draws <paramref name="numOfCards"/> random cards from the undealt deck and subsequently removes them.
    /// </summary>
    /// <param name="numOfCards">Number of cards to draw and return</param>
    /// <returns>A list containing all the drawn cards</returns>
    public List<Card> DrawCards(int numOfCards)
    {
        return game.DrawCardsFromDeck(numOfCards, DeckChoices.UNDEALT);
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
