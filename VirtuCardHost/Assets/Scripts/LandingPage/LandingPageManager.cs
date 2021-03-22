using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;
using PhotonScripts;
using System;

public class LandingPageManager : MonoBehaviour
{
    // this dropdown should be populated with all the game names
    public Dropdown gameChoiceDropdown;

    // this is the UI toggle component that allows the host to join or not
    public Toggle canHostJoinToggle;

    // this is the button that is pressed when a user wants to create a game
    public Button createGameBtn;

    // Start is called before the first frame update
    void Start()
    {
        canHostJoinToggle.SetIsOnWithoutNotify(HostData.CanHostJoinGame());
        // add event listener for the value changing
        canHostJoinToggle.onValueChanged.AddListener(
            delegate { CanHostJoinToggleValueChanged(canHostJoinToggle.isOn); });

        string[] gameNames = Enum.GetNames(typeof(GameTypes));
        foreach (string gameName in gameNames) {
            gameChoiceDropdown.options.Add(new Dropdown.OptionData(gameName));
        }
        //gameChoiceDropdown.options.Add(new Dropdown.OptionData("Freeplay"));
        //gameChoiceDropdown.options.Add(new Dropdown.OptionData("Uno"));
        //gameChoiceDropdown.options.Add(new Dropdown.OptionData("Go Fish"));

        gameChoiceDropdown.onValueChanged.AddListener(GameChoiceValueChanged);

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// OnCreateButtonClick()
    /// 
    /// Function triggered by 'Create Button' click. Generates RoomCode string.
    public void OnCreateButtonClick()
    {
        //PhotonNetwork.ConnectUsingSettings();    //Connecting to Photon Master Servers
        if (PhotonNetwork.IsConnectedAndReady)
        {
            HostData.SetGame((GameTypes) Enum.Parse(typeof(GameTypes),
                gameChoiceDropdown.options[gameChoiceDropdown.value].text));

            string RoomCode = NetworkController.generateCode(); //Generating Room Code string and storing it
            HostData.setMaxNumPlayers(HostData.GetGame().GetMaximumNumOfPlayers());
            NetworkController.CreateAndJoinRoom(RoomCode, HostData.GetGame().GetMaximumNumOfPlayers()); //Creating Photon Room with Generated Code

            HostData.setJoinCode(RoomCode);
            SceneManager.LoadScene(SceneNames.WaitingRoomScreen, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Not connected to Photon.");
        }
    }

    private void GameChoiceValueChanged(int state)
    {
        HostData.setSelectedGame(state);
    }

    /// <summary>
    /// This is the callback for when the canHostJoinToggle's value is changed.
    /// It sets the corresponding boolean in the HostData class
    /// </summary>
    /// <param name="state"></param>
    private void CanHostJoinToggleValueChanged(bool state)
    {
        HostData.setCanHostJoinGame(state);
    }
}