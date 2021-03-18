using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoFish : Game
{
    public GoFish()
    {
        SetGameName(Enum.GetName(typeof(GameTypes), GameTypes.GoFish));
    }

    /// <summary>
    /// This method initializes the game
    /// </summary>
    public override void InitializeGame()
    {
    }

    /// <summary>
    /// This method is used to verify that the Card that the player wants to play is valid.
    /// It does NOT actually play the card, it only checks if it is possible
    /// </summary>
    /// <param name="cardToPlay"></param>
    /// <returns>True or false depending on validity of move</returns>
    public override bool VerifyMove(Card cardToPlay)
    {
        return true;
    }

    /// <summary>
    /// This method is used to verify that the player can skip their turn
    /// It does NOT actually skip their turn, it only checks if it is possible
    /// </summary>
    /// <param name="playerIndex">The index of the player</param>
    /// <returns>True or false depending on validity of skip</returns>
    public override bool VerifyCanSkip(int playerIndex)
    {
        return true;
    }

    /// <summary>
    /// This method is called when a player plays a card
    /// </summary>
    /// <param name="cardToPlay">Card that the player is playing</param>
    /// <param name="playerIndex">index of the player</param>
    /// <returns></returns>
    public override bool DoMove(Card cardToPlay, int playerIndex)
    {
        return true;
    }

    /// <summary>
    /// This is the method that is called when a player has their turn forcefully skipped by the timer
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void ForceSkipTurn(int playerIndex)
    {
        
    }
}
