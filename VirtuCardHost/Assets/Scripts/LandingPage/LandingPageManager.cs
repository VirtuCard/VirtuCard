using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Freeplay"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Uno"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Go Fish"));

        gameChoiceDropdown.onValueChanged.AddListener(GameChoiceValueChanged);

    
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnCreateButtonClick()
    {
        HostData.setJoinCode("EFADDS");
        SceneManager.LoadScene(SceneNames.WaitingRoomScreen, LoadSceneMode.Single);
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