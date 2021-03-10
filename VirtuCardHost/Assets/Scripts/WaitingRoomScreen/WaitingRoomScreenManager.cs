using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        canHostJoinToggle.onValueChanged.AddListener(
            delegate { CanHostJoinToggleValueChanged(canHostJoinToggle.isOn); });
        numPlayers.onValueChanged.AddListener(OnNumPlayersFieldChange);

        settingsPanel.SetActive(false);
        joinCode.text = HostData.GetJoinCode();
        selectedGame.text = HostData.GetSelectedGame();
        currPlayerCount.text = "0 players";
        canHostJoinToggle.isOn = HostData.CanHostJoinGame();
        numPlayers.SetTextWithoutNotify(HostData.GetMaxNumPlayers().ToString());
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