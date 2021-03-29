using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileScreenController : MonoBehaviour
{
    public Text nameText;
    public Text namePlaceholderText;
    public Text usernameText;
    public Text usernamePlaceholderText;
    public InputField nameInput;
    public InputField usernameInput;
    public Text emailText;
    public Text gamesPText;
    public Text sceneHeadingText;
    public Text gamesWText;
    public Text gamesLText;
    public Image avatarImage;
    public Button editButton;
    public Button backButton;
    public Text errorPanelHeadingText;
    public Text errorPanelMessageText;
    public GameObject errorPanel;
    public Text buttonText;

    private bool isAnonymous;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseScripts.User user = ClientData.UserProfile;
        isAnonymous = user.IsAnonymous;
        sceneHeadingText.text += user.Username;
        sceneHeadingText.text += user.Username.EndsWith("s") ? ("' Profile") : ("'s Profile");
        nameText.text += user.Name;
        namePlaceholderText.text += user.Name;
        usernameText.text += user.Username;
        usernamePlaceholderText.text += user.Username;
        emailText.text += user.Email;
        gamesPText.text += user.GamesPlayed;
        gamesWText.text += user.GamesWon;
        gamesLText.text += user.GamesLost;
    }
    
    public void OnEditBtnClicked()
    {
        if (isAnonymous)
        {
            errorPanel.SetActive(true);
            return;
        }
        
        nameText.gameObject.SetActive(false);
        usernameText.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(true);
        usernameInput.gameObject.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        if ((usernameInput.text == "" || usernameInput.text == usernameText.text) &&
            (nameInput.text == "" || nameInput.text == nameText.text))
        {
            SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
        }
        else
        {
            errorPanel.SetActive(true);
            errorPanelHeadingText.text = "Unsaved Changes";
            errorPanelMessageText.text = "You have some unsaved changes to your profile data. Are you sure you want to go back?";
        }
    }
}
