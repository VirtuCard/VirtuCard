using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientData
{
    private static bool isCurrentlyTurn = true;

    public static void setCurrentTurn(bool value)
    {
        isCurrentlyTurn = value;
    }

    public static bool isCurrentTurn()
    {
        return isCurrentlyTurn;
    }
}