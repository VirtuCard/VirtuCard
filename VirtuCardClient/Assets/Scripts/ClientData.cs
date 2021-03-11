using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class ClientData
{
    private static string joinCode;
    private static bool isCurrentlyTurn = false;
    private static bool chatAllowed;
    private static bool hostCanJoin;

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
    
    public static void FromHashtable(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged == null)
        {
            return;
        }

        if (propertiesThatChanged.ContainsKey("ChatAllowed"))
        {
            setChatAllowed((Boolean) propertiesThatChanged["ChatAllowed"]);
        }

        if (propertiesThatChanged.ContainsKey("HostCanJoin"))
        {
            hostCanJoin = ((Boolean) propertiesThatChanged["HostCanJoin"]);
            Debug.Log("Hello " + hostCanJoin);
        }
    }
}