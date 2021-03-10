using UnityEngine;
using UnityEngine.UI;
using FirebaseScripts;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Realtime;
//using Photon.Pun;

public class JoinGameMethod : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// This method grabs the code that the user input
    /// then puts it in to the code to join a Photon room.
    /// <summary>

    public string joinCode;
    public GameObject inputField;
    public GameObject errorCode;
    bool successfulConnect = true;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public void ConnectClientClicked()
    {
        joinCode = inputField.GetComponent<Text>().text;
        Debug.Log("Join Code is: " + joinCode);
        ConnectClientToServer(joinCode);
    }

    /// <summary>
    /// This method connects the client to the server
    /// </summary>
    /// <param name="code">The Join Code of the room</param>
    private void ConnectClientToServer(string code)
    {
        // ----- EXAMPLE ------
        // This is an example of how you would join the room
        PhotonNetwork.JoinRoom(code);
        //object[] content = new object[] {"hello darkness"};
        //OnPhotonJoinRoomFailed(content, "shot");
        
        if (successfulConnect)
        {

        }
        else
        {
            errorCode.GetComponent<Text>().text = "Joining room failed!";
        }
    }

    void OnPhotonJoinRoomFailed(object[] codeAndMsg, string message)
    {
        Debug.Log("testing");
        successfulConnect = false;
        errorCode.GetComponent<Text>().text = "Joining room failed!";
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnSignalSent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnSignalSent;
    }

    private void OnSignalSent(EventData photonEvent)
    {
       // Every photon event has its own unique code, I've chosen
       // 1 as the one to work with the initial room information return
       if (photonEvent.Code == 1){
            object[] data = (object[])photonEvent.CustomData;
            string s = (string)data[0];
            bool test = (bool)data[1];
            int players = (int)data[2];
            Debug.Log(s);
            Debug.Log(test);
            Debug.Log(players);

            if (!test)
            {
                errorCode.GetComponent<Text>().text = "Host cannot join!";
            }
        }
    }
    private void DoSomething()
    {
        object[] content = new object[] {"hello darkness"};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendUnreliable);
    }
}