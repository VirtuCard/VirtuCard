using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseScripts;
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
    public GameObject errorPanel;
    public GameObject anonymousErrorPanel;
    public GameObject successPanel;
    public GameObject incompleteErrorPanel;
    public Text errorPanelHeadingText;
    public Text errorPanelMessageText;

    private bool isAnonymous;
    public static int success = 0;

    // Start is called before the first frame update
    void Start()
    {
        User user = ClientData.UserProfile;
        editButtonText.text = "Edit";
        errorPanelHeadingText.text = "Anonymous User";
        errorPanelMessageText.text = "Anonymous user cannot edit profile data";
        errorPanel.SetActive(false);
        successPanel.SetActive(false);
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

    // Update is called once per frame
    void Update()
    {
        if (success == 1)
        {
            successPanel.SetActive(true);
        }
        else if (success == -1)
        {
            errorPanel.SetActive(true);
            errorPanelHeadingText.text = "Username in use";
            errorPanelMessageText.text = "Please enter another username.";
        }
        else if (success == -2)
        {
            errorPanel.SetActive(true);
        }
        else
        {
            errorPanel.SetActive(false);
            successPanel.SetActive(false);
        }
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
            if (usernameInputText.text.Equals("") && nameInputText.text.Equals(""))
            {
                success = -2;
                errorPanel.SetActive(true);
                errorPanelHeadingText.text = "Username and Name Blank";
                errorPanelMessageText.text = "Please enter a valid username and player name.";
                return;
            }
            if (usernameInputText.text.Equals(""))
            {
                success = -2;
                errorPanel.SetActive(true);
                errorPanelHeadingText.text = "Username Blank";
                errorPanelMessageText.text = "Please enter a valid username.";
                return;
            }
            if (nameInputText.text.Equals(""))
            {
                success = -2;
                errorPanel.SetActive(true);
                errorPanelHeadingText.text = "Player Name Blank";
                errorPanelMessageText.text = "Please enter a valid player name.";
                return;
            }
            string newUsername = usernameInputText.text;
            string newName = nameInputText.text;
            DatabaseUtils.findUsername(newUsername, val =>
            {
                if (val != null)
                {
                    success = -1;
                    return;
                }
                else
                {
                    success = 1;
                    ClientData.UserProfile.Username = usernameInputText.text;
                    ClientData.UserProfile.Name = nameInputText.text;
                    DatabaseUtils.updateUser(ClientData.UserProfile, b => { Debug.Log("Updated username and name"); });
                }
            });
        }
  
    }

    public void onOKButtonClicked()
    {
        success = 0;
        SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
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

    public void onErrorPanelClose()
    {
        success = 0;
    }
}
