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

public class JoinGameMethod : MonoBehaviour
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
        if (successfulConnect)
        {

        }
        else
        {
            errorCode.GetComponent<Text>().text = "Joining room failed!";
        }
    }

    void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        successfulConnect = false;
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
       if (photonEvent.Code == 1){
            string s = (string) photonEvent[0];
            Debug.Log(s);
        }
    }
    private void DoSomething()
    {
        object[] content = new object[] {"hello darkness"};
        //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

}