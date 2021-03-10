using UnityEngine;
using UnityEngine.UI;
using FirebaseScripts;
using UnityEngine.SceneManagement;
using Photon.Pun;

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

}