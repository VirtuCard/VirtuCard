using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PhotonScripts;

public class WaitingRoomScreenManager : MonoBehaviour
{
    //Settings
    public GameObject settingsPanel;
    public InputField numPlayers;
    public Toggle canHostJoinToggle;

    //Displayed Codes
    public Text joinCode;
    public Text selectedGame;
    public Text currPlayerCount;

    // GameObjects for the player list feature
    public GameObject textBoxTemplate;
    private ArrayList playerList;

    // Start is called before the first frame update
    void Start()
    {
        canHostJoinToggle.onValueChanged.AddListener(
            delegate { CanHostJoinToggleValueChanged(canHostJoinToggle.isOn); });
        numPlayers.onValueChanged.AddListener(OnNumPlayersFieldChange);
        playerList = NetworkController.ListAllPlayers();
        CreatePlayerList();
        settingsPanel.SetActive(false);
        joinCode.text = HostData.GetJoinCode();
        selectedGame.text = HostData.GetSelectedGame();
        currPlayerCount.text = "0 players";
        canHostJoinToggle.isOn = HostData.CanHostJoinGame();
        numPlayers.SetTextWithoutNotify(HostData.GetMaxNumPlayers().ToString());
    }

    public void CreatePlayerList()
    {
        for(int i = 0; i < 10; ++i)
        {
            // Creates a text game object based on the template based as an argument
            GameObject textBox = Instantiate(textBoxTemplate) as GameObject;
            
            // Setting the text game object to active
            textBox.SetActive(true);
            
            // Setting the text in the text game object
            textBox.GetComponent<Text>().text = "" + i;
            
            // Assigning the parent of text game object as the parent of template 
            // the second argument indicates whether the game object will be placed automatically in the game world
            // since we want it to be set to that of the parent, we pass 'false' as the second argument
            textBox.transform.SetParent(textBoxTemplate.transform.parent, false);
        }
    }

    public void OnClickCloseOptions()
    {
        numPlayers.SetTextWithoutNotify(HostData.GetMaxNumPlayers().ToString());
        settingsPanel.SetActive(false);
    }


    public void OnClickGameExit()
    {
        //Implement Close Game lobby code here.
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

    // Update is called once per frame
    void Update()
    {
        joinCode.text = HostData.GetJoinCode();
        //Refresh players list here
    }
}