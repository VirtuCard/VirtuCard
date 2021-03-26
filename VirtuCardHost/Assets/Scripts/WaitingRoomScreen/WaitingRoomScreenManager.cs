using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using PhotonScripts;
using Photon.Pun;
using System;
using Photon.Realtime;

public class WaitingRoomScreenManager : MonoBehaviour
{
    //Settings
    public GameObject settingsPanel;
    public InputField numPlayers;
    public Toggle canHostJoinToggle;

    public Toggle chatEnabledToggle;

    //Freeplay settings
    public GameObject freeplaySettingsPanel;
    public Toggle enableHearts;
    public Toggle enableClubs;
    public Toggle enableSpades;
    public Toggle enableDiamonds;
    public Toggle showLastCard;
    public Button FreeplaySettingsButton;

    // Timer Settings
    public Toggle timerEnabledToggle;
    public InputField minutesInput;
    public InputField secondsInput;

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

        //  Freeplay listeners
        enableHearts.onValueChanged.AddListener(
            delegate { HeartsToggleValueChanged(enableHearts.isOn); });
        enableClubs.onValueChanged.AddListener(
            delegate { ClubsToggleValueChanged(enableClubs.isOn); });
        enableSpades.onValueChanged.AddListener(
            delegate { SpadesToggleValueChanged(enableSpades.isOn); });
        enableDiamonds.onValueChanged.AddListener(
            delegate { DiamondsToggleValueChanged(enableDiamonds.isOn); });
        // setup initial timer stuff
        timerEnabledToggle.onValueChanged.AddListener(
            delegate { TimerToggleValueChanged(timerEnabledToggle.isOn); });
        secondsInput.text = "30";
        minutesInput.text = "1";
        secondsInput.interactable = false;
        minutesInput.interactable = false;

        numPlayers.onEndEdit.AddListener(OnNumPlayersFieldChange);
        playerList = NetworkController.ListAllPlayers();
        CreatePlayerList();
        settingsPanel.SetActive(false);
        freeplaySettingsPanel.SetActive(false);
        joinCode.text = HostData.GetJoinCode();
        selectedGame.text = HostData.GetGame().GetGameName();
        currPlayerCount.text = "0 players";
        canHostJoinToggle.isOn = HostData.CanHostJoinGame();
        chatEnabledToggle.isOn = HostData.isChatAllowed();
        numPlayers.SetTextWithoutNotify(HostData.GetGame().GetMaximumNumOfPlayers().ToString());

        //freeplay initialization
        enableHearts.isOn = HostData.getHeartsAllowed();
        enableClubs.isOn = HostData.getClubsAllowed();
        enableSpades.isOn = HostData.getSpadesAllowed();
        enableDiamonds.isOn = HostData.getDiamondsAllowed();

        startGameBtn.onClick.AddListener(delegate { StartGameBtnClicked(); });
        startGameBtn.interactable = false;
        
        //disables and hides the freeplay button if the gamemode is not freeplay
        if (HostData.GetGame().GetGameName() != "Freeplay")
        {
            //FreeplaySettingsButton.enabled = false;
            FreeplaySettingsButton.gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        joinCode.text = HostData.GetJoinCode();
        if (HostData.GetGame().GetNumOfPlayers() >= HostData.GetGame().GetMinimumNumOfPlayers())
        {
            startGameBtn.interactable = true;
        }
        else
        {
            startGameBtn.interactable = false;
        }

        //Refresh players list here
        RefreshPlayerListBox();
    }

    private void RefreshPlayerListBox()
    {
        List<string> playerNamesToAdd = new List<string>();
        List<bool> boxIsAssociatedWithConnectedPlayer = new List<bool>();

        for (int x = 0; x < textBoxes.Count; x++)
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

    /// <summary>
    /// This method is the event handler for when the timer enabled toggle is changed
    /// </summary>
    /// <param name="isOn"></param>
    private void TimerToggleValueChanged(bool isOn)
    {
        secondsInput.interactable = isOn;
        minutesInput.interactable = isOn;
        HostData.SetIsTimerEnabled(isOn);
    }

    /// <summary>
    /// This method is called whenever the seconds input is updated
    /// </summary>
    public void SecondsInputUpdated()
    {
        int second = int.Parse(secondsInput.text);
        if (second < 0)
        {
            secondsInput.text = "0";
        }
        else if (second > 59)
        {
            secondsInput.text = "59";
        }
    }

    /// <summary>
    /// This method is called whenever the minutes input is updated
    /// </summary>
    public void MinutesInputUpdated()
    {
        int minute = int.Parse(minutesInput.text);
        if (minute < 0)
        {
            minutesInput.text = "0";
        }
        else if (minute > 9)
        {
            minutesInput.text = "9";
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
        HostData.SetIsTimerEnabled(timerEnabledToggle.isOn);
        if (timerEnabledToggle.isOn)
        {
            HostData.SetTimerSeconds(int.Parse(secondsInput.text));
            HostData.SetTimerMinutes(int.Parse(minutesInput.text));
        }
        else
        {
            HostData.SetTimerSeconds(-1);
            HostData.SetTimerMinutes(-1);
        }

        List<object> content = new List<object>();

        // add timer stuff to content
        content.Add(HostData.IsTimerEnabled());
        content.Add(HostData.GetTimerSeconds());
        content.Add(HostData.GetTimerMinutes());

        // add player names to content
        List<PlayerInfo> allConnectedPlayers = HostData.GetGame().GetAllPlayers();
        content.Add(allConnectedPlayers.Count);
        for (int x = 0; x < allConnectedPlayers.Count; x++)
        {
            content.Add(allConnectedPlayers[x].username);
        }

        // send content
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(6, content.ToArray(), raiseEventOptions, SendOptions.SendUnreliable);

        // change the scene to the gamescreen
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

//Freeplay options methods

    public void OnClickBackOnFreeplaySettings()
    {
        settingsPanel.SetActive(true);
        freeplaySettingsPanel.SetActive(false);
    }

    public void OnClickFreeplaySettings()
    {
        //Add logic to check what game mode is selected
        //TODO

        freeplaySettingsPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void HeartsToggleValueChanged(bool isOn)
    {
        HostData.setHeartsAllowed(isOn);
    }

    private void ClubsToggleValueChanged(bool isOn)
    {
        HostData.setClubsAllowed(isOn);
    }

    private void SpadesToggleValueChanged(bool isOn)
    {
        HostData.setSpadesAllowed(isOn);
    }
       
    private void DiamondsToggleValueChanged(bool isOn)
    {
        HostData.setDiamondsAllowed(isOn);
    }

    private void LastCardToggleChanged(bool isOn)
    {
        HostData.setDisplayLastCard(isOn);
    }

//End freeplay options


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
            HostData.setMaxNumPlayers(HostData.GetGame().GetMaximumNumOfPlayers());
            numPlayers.SetTextWithoutNotify(HostData.GetGame().GetMaximumNumOfPlayers().ToString());
            return;
        }

        int value = 0;
        if (!int.TryParse(newValue, out value))
        {
            HostData.setMaxNumPlayers(HostData.GetGame().GetMaximumNumOfPlayers());
            numPlayers.SetTextWithoutNotify(HostData.GetGame().GetMaximumNumOfPlayers().ToString());
        }
        else
        {
            HostData.setMaxNumPlayers(value);
            numPlayers.SetTextWithoutNotify(HostData.GetMaxNumPlayers().ToString());
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