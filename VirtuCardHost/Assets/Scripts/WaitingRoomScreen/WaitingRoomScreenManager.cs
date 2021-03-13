using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using PhotonScripts;
using Photon.Pun;
using Photon.Realtime;

public class WaitingRoomScreenManager : MonoBehaviour
{
    //Settings
    public GameObject settingsPanel;
    public InputField numPlayers;
    public Toggle canHostJoinToggle;
    public Toggle chatEnabledToggle;

    //Displayed Codes
    public Text joinCode;
    public Text selectedGame;
    public Text currPlayerCount;

    // GameObjects for the player list feature
    public GameObject textBoxTemplate;
    private ArrayList playerList;
    private List<GameObject> textBoxes = new List<GameObject>();
    public Button startGameBtn;

    // Start is called before the first frame update
    void Start()
    {
        canHostJoinToggle.onValueChanged.AddListener(
            delegate { CanHostJoinToggleValueChanged(canHostJoinToggle.isOn); });
        chatEnabledToggle.onValueChanged.AddListener(
            delegate { ChatToggleValueChanged(chatEnabledToggle.isOn); });
        numPlayers.onValueChanged.AddListener(OnNumPlayersFieldChange);
        playerList = NetworkController.ListAllPlayers();
        CreatePlayerList();
        settingsPanel.SetActive(false);
        joinCode.text = HostData.GetJoinCode();
        selectedGame.text = HostData.GetGame().GetGameName();
        currPlayerCount.text = "0 players";
        canHostJoinToggle.isOn = HostData.CanHostJoinGame();
        chatEnabledToggle.isOn = HostData.isChatAllowed();
        numPlayers.SetTextWithoutNotify(HostData.GetMaxNumPlayers().ToString());


        startGameBtn.onClick.AddListener(delegate { StartGameBtnClicked(); });
        startGameBtn.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        joinCode.text = HostData.GetJoinCode();
        if (HostData.GetGame().GetNumOfPlayers() > 0)
        {
            startGameBtn.interactable = true;
        }

        //Refresh players list here
        RefreshPlayerListBox();
    }

    private void RefreshPlayerListBox()
    {
        List<string> playerNamesToAdd = new List<string>();
        List<bool> boxIsAssociatedWithConnectedPlayer = new List<bool>();

        for(int x = 0; x < textBoxes.Count; x++)
        {
            boxIsAssociatedWithConnectedPlayer.Add(false);
        }

        var allConnectedPlayers = HostData.GetGame().GetAllPlayers();
        currPlayerCount.text = allConnectedPlayers.Count + " players";

        foreach (PlayerInfo player in allConnectedPlayers)
        {
            bool playerHasBeenAddedAlready = false;
            if (textBoxes.Count > 0)
            {
                for (int x = 0; x < textBoxes.Count; x++)
                {
                    string boxName = textBoxes[x].name;
                    if (boxName.Equals(player.photonPlayer.NickName))
                    {
                        // do nothing the player has already been added
                        playerHasBeenAddedAlready = true;
                        boxIsAssociatedWithConnectedPlayer[x] = true;
                        break;
                    }
                }
            }
            if (!playerHasBeenAddedAlready)
            {
                playerNamesToAdd.Add(player.photonPlayer.NickName);
            }
        }

        for (int x = boxIsAssociatedWithConnectedPlayer.Count - 1; x >= 0; x--)
        {
            if (boxIsAssociatedWithConnectedPlayer[x] == false)
            {
                // this player disconnected, so remove them from list
                var box = textBoxes[x];
                textBoxes.RemoveAt(x);
                Destroy(box);
            }
        }

        foreach (string name in playerNamesToAdd)
        {
            AddNameToPlayerListBox(name);
        }
    }

    private void AddNameToPlayerListBox(string name)
    {
        // Creates a text game object based on the template based as an argument
        GameObject textBox = Instantiate(textBoxTemplate) as GameObject;

        // Setting the text game object to active
        textBox.SetActive(true);

        // Setting the text and name in the text game object
        textBox.name = name;
        textBox.GetComponent<Text>().text = name;

        // Assigning the parent of text game object as the parent of template 
        // the second argument indicates whether the game object will be placed automatically in the game world
        // since we want it to be set to that of the parent, we pass 'false' as the second argument
        textBox.transform.SetParent(textBoxTemplate.transform.parent, false);

        // Adding game object to list
        textBoxes.Add(textBox);
    }

    public void StartGameBtnClicked()
    {
        // send an event to clients telling them to start their games
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(6, null, raiseEventOptions, SendOptions.SendUnreliable);
        HostData.GetGame().PrintAllPlayers();
        SceneManager.LoadScene(SceneNames.GameScreen, LoadSceneMode.Single);
    }

    public void CreatePlayerList()
    {
        Debug.Log("Creating player list");

        // Maintains a list of all the new text objects
        textBoxes = new List<GameObject>();

        // If the list is not empty, then the list is cleared to account for player leaving the room
        if (textBoxes.Count > 0)
        {
            foreach (GameObject box in textBoxes)
            {
                Destroy(box);
            }

            textBoxes.Clear();
        }

        var playerList = HostData.GetGame().GetAllPlayers();
        // Creating the list from scratch from the returned list of names (PhotonNetwork.PlayerList)
        foreach (var player in playerList)
        {
            // Creates a text game object based on the template based as an argument
            GameObject textBox = Instantiate(textBoxTemplate) as GameObject;

            // Setting the text game object to active
            textBox.SetActive(true);

            // Setting the text and name in the text game object
            textBox.name = player.photonPlayer.NickName;
            textBox.GetComponent<Text>().text = player.photonPlayer.NickName;

            // Assigning the parent of text game object as the parent of template 
            // the second argument indicates whether the game object will be placed automatically in the game world
            // since we want it to be set to that of the parent, we pass 'false' as the second argument
            textBox.transform.SetParent(textBoxTemplate.transform.parent, false);

            // Adding game object to list
            textBoxes.Add(textBox);
        }
    }

    public void OnClickCloseOptions()
    {
        numPlayers.SetTextWithoutNotify(HostData.GetMaxNumPlayers().ToString());
        string updatedPlayers = HostData.GetMaxNumPlayers().ToString();
        // Sending signal to clients to update their waiting screen
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        object[] content = new object[] {updatedPlayers};
        PhotonNetwork.RaiseEvent(10, content, raiseEventOptions, SendOptions.SendUnreliable);
        settingsPanel.SetActive(false);
    }


    public void OnClickGameExit()
    {
        //Implement Close Game lobby code here.
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(SceneNames.LandingPage, LoadSceneMode.Single);
    }

    private void OnNumPlayersFieldChange(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }

        int value = 0;
        if (!int.TryParse(newValue, out value))
        {
            HostData.setMaxNumPlayers(3);
        }
        else
        {
            HostData.setMaxNumPlayers(value);
        }
    }

    private void CanHostJoinToggleValueChanged(bool state)
    {
        HostData.setCanHostJoinGame(state);
    }

    private void ChatToggleValueChanged(bool state)
    {
        HostData.setChatAllowed(state);
    }
}