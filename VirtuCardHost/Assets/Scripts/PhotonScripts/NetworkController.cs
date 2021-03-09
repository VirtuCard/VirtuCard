using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Text;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private int RoomCodeLength = 6;

    //Field for Host's RoomCode.
    //Format: ABCDEF
    private string RoomCode = "";

    //Field to store the name of the file
    // that holds the RoomCode for testing purposes
    private string RoomCodeFileName = "RoomCode.txt";

    /* Start is called before the first frame update
    public static void OnCreateButtonClick()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon Master Servers
    
    } */

    public override void OnConnectedToMaster()
    {
        Debug.Log("The Host is now connected to the " + PhotonNetwork.CloudRegion + " server.");
        Debug.Log("Generated Room Code is " + RoomCode);
    }

    /// generateCode()
    /// 
    /// Method to generate Host's RoomCode.
    /// Format: ABCDEF
    /// 
    /// <returns> RoomCode String </returns>
    public static string generateCode()
    {
        //Integer Array used to create RoomCode

        int RoomCodeLength = 6;
        int[] RoomCodeNum = new int[RoomCodeLength];

        //Placing Numbers to act as 'pseudo' letters. Each number
        // corresponds to a letter. For example, 0 is A.
        for (int i = 0; i < RoomCodeLength; i++)
        {
            RoomCodeNum[i] = Random.Range(65, 90);
        }

        //Coverting Numbers into ASCII and then appending them to
        // roomCodeBuffer
        string roomCodeBuffer = "";
        for (int i = 0; i < RoomCodeLength; i++)
        {
            roomCodeBuffer += (char) (RoomCodeNum[i]);
        }

        //Writing Room Code to file
        WriteRoomCodeToFile(roomCodeBuffer);

        //Setting Host's RoomCode field to generated roomCodeBuffer
        //RoomCode = roomCodeBuffer;
        return roomCodeBuffer;
    }

    // Function to Create and Join a Room with associated Room Code
    public static void CreateAndJoinRoom(string RoomCodeString)
    {
        // Sets the max players that can join to 10
        // Number will change depending on the game
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;

        // This will join the room depending on the roomcode
        // If room doesn't exist, it creates the room
        PhotonNetwork.CreateRoom(RoomCodeString, options, null);
    }

    /// WriteRoomCodeToFile()
    /// 
    /// Function used to write the Room Code to a File
    static void WriteRoomCodeToFile(string RoomCode)
    {
        File.WriteAllText("RoomCode.txt", RoomCode);
    }

   // Function that returns a list of all the players in a room
   public ArrayList ListAllPlayers()
    {
        ArrayList playerList = new ArrayList();
        foreach(var player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player.NickName);
        }
        Debug.Log(playerList.ToString());
        return playerList;
    }
}