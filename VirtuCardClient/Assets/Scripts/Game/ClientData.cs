using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientData
{
    private static bool isCurrentlyTurn = false;
    private static bool chatAllowed;

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
}