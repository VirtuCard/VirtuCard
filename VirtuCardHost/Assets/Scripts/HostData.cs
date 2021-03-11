using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    
    public static void SetGame(GameTypes gameType)
    {
        string gameName = Enum.GetName(typeof(GameTypes), gameType);
        if (gameName == "TestGame")
        {
            currentGame = new TestGame();
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
        if (numPlayers > 12)
        {
            maxNumPlayers = 12;
        } else if (numPlayers < 3)
        {
            maxNumPlayers = 3;
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
    
    public static Hashtable ToHashtable()
    {
        Hashtable table = new Hashtable();
        table.Add("ChatAllowed", chatAllowed);
        table.Add("HostCanJoin", canHostJoinGame);
        Debug.Log(table.ToString());
        return table;
    }

}
