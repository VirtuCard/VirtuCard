using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class ClientData
{
    private static string joinCode;
    private static string currentPlayerTurn;
    private static bool isCurrentlyTurn = false;
    private static bool chatAllowed = true;
    private static bool hostCanJoin;
    private static string gameName = "";

    private static List<string> playerNames = new List<string>();
    
    public static List<string> GetAllConnectedPlayers()
    {
        return playerNames;
    }
    public static void RemoveConnectedPlayerName(string nameToRemove)
    {
        playerNames.Remove(nameToRemove);
    }
    public static void AddConnectedPlayerName(string newName)
    {
        playerNames.Add(newName);
    }

    public static void SetGameName(string name)
    {
        gameName = name;
    }
    public static string GetGameName()
    {
        return gameName;
    }

    public static bool canHostJoin()
    {
        return hostCanJoin;
    }
    
    public static void setJoinCode(string value)
    {
        joinCode = value;
    }

    public static string getJoinCode()
    {
        return joinCode;
    }

    public static void setCurrentTurn(bool value)
    {
        isCurrentlyTurn = value;
    }

    public static bool isCurrentTurn()
    {
        return isCurrentlyTurn;
    }

    public static bool isChatAllowed()
    {
        return chatAllowed;
    }

    public static void setChatAllowed(bool isChatAllowed)
    {
        chatAllowed = isChatAllowed;
    }

    public static string getCurrentPlayerTurn()
    {
        return currentPlayerTurn;
    }

    public static void setCurrentPlayerTurn(string name)
    {
        currentPlayerTurn = name;
    }

    public static void FromHashtable(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged == null)
        {
            return;
        }

        if (propertiesThatChanged.ContainsKey("ChatAllowed"))
        {
            //setChatAllowed((Boolean) propertiesThatChanged["ChatAllowed"]);
            chatAllowed = ((Boolean) propertiesThatChanged["ChatAllowed"]);
            Debug.Log("Chat is changed from main and it is " + ((Boolean) propertiesThatChanged["ChatAllowed"]));
        }

        if (propertiesThatChanged.ContainsKey("HostCanJoin"))
        {
            hostCanJoin = ((Boolean) propertiesThatChanged["HostCanJoin"]);
            Debug.Log("Hello " + hostCanJoin);
        }
        
    }
}