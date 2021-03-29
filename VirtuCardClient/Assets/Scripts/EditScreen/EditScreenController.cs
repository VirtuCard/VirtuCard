using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditScreenController : MonoBehaviour
{
    public Text nameText;
    public Text usernameText;
    public Text emailText;
    public Input nameInput;
    public Input usernameInput;
    public Text gamesPText;
    public Text sceneHeadingText;
    public Text gamesWText;
    public Text gamesLText;
    public Image avatarImage;
    public Button saveButton;
    public GameObject errorPanel;

    private bool isAnonymous;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseScripts.User user = ClientData.UserProfile;
        isAnonymous = user.IsAnonymous;
        sceneHeadingText.text += user.Username;
        sceneHeadingText.text += user.Username.EndsWith("s") ? ("' Profile") : ("'s Profile");
        nameText.text += user.Name;
        usernameText.text += user.Username;
        emailText.text += user.Email;
        gamesPText.text += user.GamesPlayed;
        gamesWText.text += user.GamesWon;
        gamesLText.text += user.GamesLost;
    }

    public void OnSaveBtnClicked()
    {
        if (isAnonymous)
        {
            errorPanel.SetActive(true);
            return;
        }
        SceneManager.LoadScene(SceneNames.EditPage, LoadSceneMode.Single);
    }
}

