using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to store host data between scenes. It allows the data to be persistent
/// </summary>
public static class HostData
{
    private static bool canHostJoinGame;
    public static bool CanHostJoinGame()
    {
        return canHostJoinGame;
    }
    public static void setCanHostJoinGame(bool state)
    {
        canHostJoinGame = state;
    }
}
