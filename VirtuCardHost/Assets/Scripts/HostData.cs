using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FirebaseScripts;
using Photon.Pun;
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
    private static User userProfile;

    public static User UserProfile
    {
        get => userProfile;
        set => userProfile = value;
    }

    private static bool isTimerEnabled;
    private static int timerMinutes;
    private static int timerSeconds;

    //Adding more settings for freeplay customizability
    private static bool areHeartsAllowed = true;
    private static bool areClubsAllowed = true;
    private static bool areSpadesAllowed = true;
    private static bool areDiamondsAllowed = true;
    private static bool displayLastCard = true;
    // eventually we will add more functionality to freeplay mode but this will do for now
    
    public static void SetGame(GameTypes gameType)
    {
        string gameName = Enum.GetName(typeof(GameTypes), gameType);
        if (gameName == "TestGame")
        {
            currentGame = new TestGame();
            return;
        }
        else if (gameName == "GoFish")
        {
            currentGame = new GoFish();
            return;
        }
        else if (gameName == "Freeplay")
        {
            currentGame = new Freeplay();
            return;
        }
        else if (gameName == "War")
        {
            currentGame = new War();
            return;
        }
        /* Here is a sample to add a new game
        else if (gameName == "<insert_other_game>")
        {
            currentGame = new <other_game>();
            return;
        }
        */
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
        PhotonNetwork.CurrentRoom.SetCustomProperties(ToHashtable());
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


    
    public static Hashtable ToHashtable()
    {
        Hashtable table = new Hashtable();
        table.Add("ChatAllowed", chatAllowed);
        table.Add("HostCanJoin", canHostJoinGame);
        Debug.Log(table.ToString());
        return table;
    }

}
