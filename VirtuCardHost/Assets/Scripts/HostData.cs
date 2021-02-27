using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to store host data between scenes. It allows the data to be persistent
/// </summary>
public static class HostData
{
    public static bool canHostJoinGame
    {
        get { return canHostJoinGame; }
        set { canHostJoinGame = value; }
    }
}
