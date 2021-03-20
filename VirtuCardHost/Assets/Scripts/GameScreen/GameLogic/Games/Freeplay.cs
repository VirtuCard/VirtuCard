using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Freeplay : Game
{
    // Start is called before the first frame update
    public Freeplay()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.Freeplay));
    }

    public override void InitializeGame()
    {
        // This eventually will be hooked up to work with the settings that the host
        // chooses on the freeplay setting screen
        CardDeck deck = CreateStandard52Deck();
        GetDeck(DeckChoices.UNDEALT).AddCards(deck);
        GetDeck(DeckChoices.UNDEALT).Print();
        AdvanceTurn(true);
    }

    public override bool VerifyMove(Card cardToPlay)
    {
        // This also will need to be updated according to the settings that the host chooses
        return true;
    }

    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        return true;
    }

    protected override void ForceSkipTurn(int playerIndex)
    {
        
    }

    public override bool VerifyCanSkip(int playerIndex)
    {
        return true;
    }

}
