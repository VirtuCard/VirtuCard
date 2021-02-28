using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private int RoomCodeLength = 6;
    //Field for Host's RoomCode.
    //Format: ABCDEF
    private string RoomCode = "";
    //Field to store the name of the file
    // that holds the RoomCode for testing purposes
    private string RoomCodeFileName = "RoomCode.txt";

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon Master Servers
        generateCode();
        WriteRoomCodeToFile();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("The Host is now connected to the " + PhotonNetwork.CloudRegion + " server.");
        Debug.Log("Generated Room Code is " + RoomCode);
    }

    // Generate Room Code Function
    void generateCode()
    {
        //Integer Array used to create RoomCode
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
            roomCodeBuffer += (char)(RoomCodeNum[i]);
        }
        //Setting Host's RoomCode field to generated roomCodeBuffer
        RoomCode = roomCodeBuffer;
    }

    // Function to Create and Join a Room with associated Room Code
    void CreateAndJoinRoom()
    {
        PhotonNetwork.CreateRoom(RoomCode);
    }

    /// WriteRoomCodeToFile()
    /// 
    /// Function used to write the Room Code to a File
    void WriteRoomCodeToFile()
    {
        File.WriteAllText(RoomCodeFileName, RoomCode);
    }

}


