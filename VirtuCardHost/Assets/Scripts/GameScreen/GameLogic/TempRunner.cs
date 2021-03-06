using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TempRunner : MonoBehaviour
{
    Game game;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(delegate { btnClick(); });
        game = new TestGame();
    }


    private void btnClick()
    {
        if (game.VerifyMove(new StandardCard(StandardCardRank.FIVE, StandardCardSuit.DIAMONDS)))
        {
            game.AddCardToDeck(new StandardCard(StandardCardRank.SIX, StandardCardSuit.DIAMONDS), DeckChoices.PLAYED);
            game.PrintDeck(DeckChoices.PLAYED);
        }
        //deck.AddCard(new StandardCard(StandardCardRank.TWO, StandardCardSuit.SPADES));
        //deck.Print();
    }
}
