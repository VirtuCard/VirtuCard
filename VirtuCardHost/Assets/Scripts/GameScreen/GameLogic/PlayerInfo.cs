using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public string username;
    public int score;
    public Photon.Realtime.Player photonPlayer;

    public override string ToString()
    {
        return "Name: \"" + username + "\" Score: " +  score;
    }
}
