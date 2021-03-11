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

    public GameObject errorPanel;
    public GameObject errorTitle;
    public GameObject errorMessage;

    public GameObject MaxPlayersText;
    public GameObject GameModeText;
    
    public static bool displayError = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this);
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
        ClientData.setJoinCode(code);
        //object[] content = new object[] {"hello darkness"};
        //OnPhotonJoinRoomFailed(content, "shot");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ClientData.setJoinCode("");
        CreateErrorMessage("Failed to Connect", "Room Code is Invalid!");
    }

    public override void OnJoinedRoom()
    {
        
        SceneManager.LoadScene(SceneNames.WaitingScreen);
    }

    void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        errorPanel.SetActive(true);
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
        if (photonEvent.Code == 1)
        {
            object[] data = (object[]) photonEvent.CustomData;
            string s = (string) data[0];
            bool hostAllowed = (bool) data[1];
            int players = (int) data[2];
            string hostName = (string) data[3];
            Debug.Log(s);
            Debug.Log(hostAllowed);
            Debug.Log(players);
            Debug.Log(hostName);

            MaxPlayersText.GetComponent<Text>().text = s;
            GameModeText.GetComponent<Text>().text = "Max Players: " + players;

            string clientName = PhotonNetwork.NickName;

            if (hostAllowed == false && clientName == hostName)
            {
                Debug.Log("Host is not allowed to join the game!");
                //errorCode.GetComponent<Text>().text = "Host cannot join!";
                PhotonNetwork.LeaveRoom();
                CreateErrorMessage("Failed to Connect", "Host is not allowed to join!");
                SceneManager.LoadScene(SceneNames.JoinGamePage);
                //displayError = true;
                
            }
        }
    }

    private void DoSomething()
    {
        object[] content = new object[] {"hello darkness"};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendUnreliable);
    }
}