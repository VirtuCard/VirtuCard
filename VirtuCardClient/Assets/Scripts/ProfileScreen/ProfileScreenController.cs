using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileScreenController : MonoBehaviour
{
    [Header("Text")]
    public Text nameText;
    public Text namePlaceholderText;
    public Text nameInputText;
    public InputField nameInput;
    public Text usernameText;
    public Text usernamePlaceholderText;
    public Text usernameInputText;
    public InputField usernameInput;
    public Text emailText;
    public Text gamesPText;
    public Text sceneHeadingText;
    public Text gamesWText;
    public Text gamesLText;
    public Image avatarImage;
    public Button editButton;
    public Button backButton;
    public Text editButtonText;

    [Header("Error Panels")]
    public GameObject anonymousErrorPanel;
    public GameObject incompleteErrorPanel;
    public Text errorPanelHeadingText;
    public Text errorPanelMessageText;
    

    private bool isAnonymous;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseScripts.User user = ClientData.UserProfile;
        editButtonText.text = "Edit";
        errorPanelHeadingText.text = "Anonymous User";
        errorPanelMessageText.text = "Anonymous user cannot edit profile data";
        anonymousErrorPanel.SetActive(false);
        incompleteErrorPanel.SetActive(false);
        nameText.gameObject.SetActive(true);
        usernameText.gameObject.SetActive(true);
        nameInput.gameObject.SetActive(false);
        usernameInput.gameObject.SetActive(false);
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
        if (editButtonText.text == "Edit")
        {
            if (isAnonymous)
            {
                anonymousErrorPanel.SetActive(true);
                return;
            }
            editButtonText.text = "Submit";
            nameText.gameObject.SetActive(false);
            usernameText.gameObject.SetActive(false);
            nameInput.gameObject.SetActive(true);
            usernameInput.gameObject.SetActive(true);
        }
        else
        {
            if (usernameInputText.Equals("") && nameInputText.Equals(""))
            {
                anonymousErrorPanel.SetActive(true);
                errorPanelHeadingText.text = "Username and Name Blank";
                errorPanelHeadingText.text = "Username and Name cannot be blank. Please enter a valid username and player name.";
                return;
            }
            if (usernameInputText.Equals(""))
            {
                anonymousErrorPanel.SetActive(true);
                errorPanelHeadingText.text = "Username Blank";
                errorPanelHeadingText.text = "Username cannot be blank. Please enter a valid username.";
                return;
            }
            if (nameInputText.Equals(""))
            {
                anonymousErrorPanel.SetActive(true);
                errorPanelHeadingText.text = "Player Name Blank";
                errorPanelHeadingText.text = "Name cannot be blank. Please enter a valid player name.";
                return;
            }
            string newUsername = usernameInputText.text;
            string newName = nameInputText.text;
            Debug.Log(newName + " " + newUsername);
        }
  
    }

    public void OnBackButtonClicked()
    {
        if ((usernameInputText.text.Equals("")) &&
            (nameInputText.text.Equals("")))
        {
            SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
        }
        else
        {
            incompleteErrorPanel.SetActive(true);
        }
    }
}
