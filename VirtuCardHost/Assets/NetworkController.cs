using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkController : MonoBehaviourPunCallbacks
{

    private int RoomCodeLength = 6;
    //Field for Host's RoomCode. In the format of ABCDEF
    public string RoomCode = "";

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon Master Servers
        generateCode();
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("The Host is now connected to the " + PhotonNetwork.CloudRegion + " server.");
        Debug.Log("Generated Room Code is " + RoomCode);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            RoomCodeNum[i] = Random.Range(0, 25);
        }
        //Coverting Numbers into ASCII and then appending them to
        // roomCodeBuffer
        string roomCodeBuffer = "";
        for (int i = 0; i < RoomCodeLength; i++)
        {
            roomCodeBuffer += (char) (RoomCodeNum[i] + 65);
        }
        //Setting Host's RoomCode field to generated roomCodeBuffer
        RoomCode = roomCodeBuffer;
    }

 
    // Function to Create and Join a Room with associated Room Code
    void CreateAndJoinRoom()
    {
        
        PhotonNetwork.CreateRoom(RoomCode);
    }

}
