using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FirebaseScripts;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


/// <summary>
/// This class is used to store host data between scenes. It allows the data to be persistent
/// </summary>
public static class HostData
{
    // just default this to true for convenience
    private static bool canHostJoinGame = true;

    private static string selectedGame = "Freeplay";
    private static int maxNumPlayers = 5;
    private static string joinCode;
    private static bool chatAllowed = true;
    private static Game currentGame;
    private static bool gameSelected = false;
    private static User userProfile;

    // last played texture stuff
    private static Texture lastPlayedCard;
    private static bool didLastPlayedCardTextureChange = false;

    public static User UserProfile
    {
        get => userProfile;
        set => userProfile = value;
    }

    private static bool isTimerEnabled;
    private static int timerMinutes;
    private static int timerSeconds;

    // for notification window popup
    private static bool doShowNotificationWindow;
    private static List<string> notificationWindowMessages = new List<string>();

    //Adding more settings for freeplay customizability
    private static bool areHeartsAllowed = true;
    private static bool areClubsAllowed = true;
    private static bool areSpadesAllowed = true;
    private static bool areDiamondsAllowed = true;
    private static bool displayLastCard = true;
    private static bool skipTurnAllowed = true;

    // eventually we will add more functionality to freeplay mode but this will do for now

    public static bool SetGame(GameTypes gameType)
    {
        if (gameSelected == false)
        {
            string gameName = Enum.GetName(typeof(GameTypes), gameType);
            if (gameName == "TestGame")
            {
                currentGame = new TestGame();
            }
            else if (gameName == "GoFish")
            {
                currentGame = new GoFish();
            }
            else if (gameName == "Freeplay")
            {
                currentGame = new Freeplay();
            }
            else if (gameName == "War")
            {
                currentGame = new War();
            }
            else
            {
                Debug.Log("Unclear Game: " + gameName);
            }

            /* Here is a sample to add a new game
            else if (gameName == "<insert_other_game>")
            {
                currentGame = new <other_game>();
                return;
            }
            */
            gameSelected = true;
            return true;
        }
        else
        {
            Debug.LogError("You have already chosen a game");
            return false;
        }
    }

    public static bool DidLastPlayedCardTextureUpdate()
    {
        return didLastPlayedCardTextureChange;
    }

    public static void SetLastPlayedCardTexture(string cardName)
    {
        didLastPlayedCardTextureChange = true;
        lastPlayedCard = Resources.Load<Texture>("Card UI/" + cardName.Trim());
    }

    public static void SetLastPlayedCardTexture(Texture texture)
    {
        didLastPlayedCardTextureChange = true;
        lastPlayedCard = texture;
    }

    public static Texture GetLastPlayedCardTexture()
    {
        didLastPlayedCardTextureChange = false;
        return lastPlayedCard;
    }

    public static void SetDoShowNotificationWindow(bool value, string message = "")
    {
        doShowNotificationWindow = value;
        if (!String.IsNullOrEmpty(message))
        {
            notificationWindowMessages.Add(message);
        }
    }

    /// <summary>
    /// This gets if the 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool GetDoShowNotificationWindow(ref string[] message)
    {
        if (doShowNotificationWindow)
        {
            message = notificationWindowMessages.ToArray();
            notificationWindowMessages.Clear();
        }

        return doShowNotificationWindow;
    }

    public static int GetTimerSeconds()
    {
        return timerSeconds;
    }

    public static void SetTimerSeconds(int seconds)
    {
        timerSeconds = seconds;
    }

    public static int GetTimerMinutes()
    {
        return timerMinutes;
    }

    public static void SetTimerMinutes(int minutes)
    {
        timerMinutes = minutes;
    }

    public static void SetIsTimerEnabled(bool isEnabled)
    {
        isTimerEnabled = isEnabled;
    }

    public static bool IsTimerEnabled()
    {
        return isTimerEnabled;
    }

    public static Game GetGame()
    {
        return currentGame;
    }

    public static string GetJoinCode()
    {
        return joinCode;
    }

    public static int GetMaxNumPlayers()
    {
        return maxNumPlayers;
    }

    public static string GetSelectedGame()
    {
        return selectedGame;
    }

    public static bool CanHostJoinGame()
    {
        return canHostJoinGame;
    }

    public static void setMaxNumPlayers(int numPlayers)
    {
        if (numPlayers > GetGame().GetMaximumNumOfPlayers())
        {
            maxNumPlayers = GetGame().GetMaximumNumOfPlayers();
        }
        else if (numPlayers < GetGame().GetMinimumNumOfPlayers())
        {
            maxNumPlayers = GetGame().GetMinimumNumOfPlayers();
        }
        else
        {
            maxNumPlayers = numPlayers;
        }
    }

    public static void setCanHostJoinGame(bool state)
    {
        canHostJoinGame = state;
    }

    public static void setSelectedGame(int state)
    {
        switch (state)
        {
            case 0:
                selectedGame = "Freeplay";
                break;
            case 1:
                selectedGame = "Uno";
                break;
            case 2:
                selectedGame = "Go Fish";
                break;
            case 3:
                selectedGame = "Freeplay";
                break;
            default:
                Debug.LogError("Got unexpected value: " + state);
                break;
        }
    }

    public static void setJoinCode(string code)
    {
        joinCode = code;
    }

    public static bool isChatAllowed()
    {
        return chatAllowed;
    }

    public static void setChatAllowed(bool isChatAllowed)
    {
        chatAllowed = isChatAllowed;
    }

    // Adding settings for freeplay

    public static void setHeartsAllowed(bool heartsAllowed)
    {
        areHeartsAllowed = heartsAllowed;
        return;
    }

    public static bool getHeartsAllowed()
    {
        return areHeartsAllowed;
    }

    public static void setClubsAllowed(bool clubsAllowed)
    {
        areClubsAllowed = clubsAllowed;
        return;
    }

    public static bool getClubsAllowed()
    {
        return areClubsAllowed;
    }

    public static void setSpadesAllowed(bool spadesAllowed)
    {
        areSpadesAllowed = spadesAllowed;
        return;
    }

    public static bool getSpadesAllowed()
    {
        return areSpadesAllowed;
    }

    public static void setDiamondsAllowed(bool diamondsAllowed)
    {
        areDiamondsAllowed = diamondsAllowed;
        return;
    }

    public static bool getDiamondsAllowed()
    {
        return areDiamondsAllowed;
    }

    public static void setDisplayLastCard(bool displayCard)
    {
        displayLastCard = displayCard;
        return;
    }

    public static bool getDisplayLastCard()
    {
        return displayLastCard;
    }

    public static void setSkipTurnAllowed(bool skipturn)
    {
        skipTurnAllowed = skipturn;
    }

    public static bool getSkipTurnAllowed()
    {
        return skipTurnAllowed;
    }


    public static bool isFreeplay()
    {
        return GetGame().GetGameName() == "Freeplay";
    }

    public static Hashtable ToHashtable()
    {
        Hashtable table = new Hashtable();
        table.Add("ChatAllowed", chatAllowed);
        table.Add("HostCanJoin", canHostJoinGame);
        table.Add("IsSkipAllowed", skipTurnAllowed);
        //Debug.Log(table.ToString());
        return table;
    }

    public static void clearGame()
    {
        HostData.GetGame().ClearAll();
        currentGame.ClearPlayers();
        currentGame = null;
        gameSelected = false;
        selectedGame = "";
        joinCode = "";
    }

    public static void resetGame()
    {
        currentGame.ClearPlayers();
    }
}