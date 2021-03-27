using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreenController : MonoBehaviour
{
    public Text nameText;
    public Text usernameText;
    public Text emailText;
    public Text gamesPText;
    public Text gamesWText;
    public Text gamesLText;
    public Image avatarImage;
    public Button editButton;
    public GameObject errorPanel;

    private bool isAnonymous;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseScripts.User user = ClientData.UserProfile;
        isAnonymous = user.IsAnonymous;
        nameText.text += user.Name;
        usernameText.text += user.Username;
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
        LoadDifferentScene loadDifferent = new LoadDifferentScene();
        loadDifferent.ChangeScene("EditScreen");
        return;
    }
}
