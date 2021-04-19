using System;
using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class ClientData
{
    private static string joinCode;
    private static string currentPlayerTurn;
    private static bool isCurrentlyTurn = false;
    private static bool chatAllowed = true;
    private static bool hostCanJoin;
    private static User userProfile;
    private static string gameName = "";
    private static string hostName = "";
    private static bool isTimerEnabled;
    private static int timerMinutes;
    private static int timerSeconds;
    private static List<string> playerNames = new List<string>();
    private static bool hideChat = false;
    private static byte[] imageData;
    private static bool isProfanityAllowed;

    public static void SetProfanityAllowed(bool value)
    {
        isProfanityAllowed = value;
    }
    public static bool IsProfanityAllowed()
    {
        return isProfanityAllowed;
    }

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

    public static User UserProfile
    {
        get => userProfile;
        set => userProfile = value;
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

    public static void setHideChat(bool isHide)
    {
        hideChat = isHide;
    }

    public static bool getHideChat()
    {
        return hideChat;
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

    public static byte[] ImageData
    {
        get => imageData;
        set => imageData = value;
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
            //Debug.Log("Chat is changed from main and it is " + ((Boolean) propertiesThatChanged["ChatAllowed"]));
        }

        if (propertiesThatChanged.ContainsKey("HostCanJoin"))
        {
            hostCanJoin = ((Boolean) propertiesThatChanged["HostCanJoin"]);
        }

        if (propertiesThatChanged.ContainsKey("IsSkipAllowed"))
        {
            GameRules.setSkipAllowed((Boolean) propertiesThatChanged["IsSkipAllowed"]);
        }
    }

    public static string HostName
    {
        get => hostName;
        set => hostName = value;
    }

    public static void ClearInformation()
    {
        playerNames = new List<string>();
        joinCode = "";
        currentPlayerTurn = "";
        isCurrentlyTurn = false;
        chatAllowed = true;
        hostCanJoin = true;
        gameName = "";
        hostName = "";
    }
}