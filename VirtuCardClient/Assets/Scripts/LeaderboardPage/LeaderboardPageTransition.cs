using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardPageTransition : MonoBehaviour
{
    public Button leaderBoardButton;
    private bool didSetLeaderboardBtn = false;

    // Start is called before the first frame update
    void Start()
    {
        leaderBoardButton.onClick.AddListener(delegate { SceneManager.LoadScene(SceneNames.LeaderboardPage, LoadSceneMode.Single); });
        didSetLeaderboardBtn = false;
    }
    private void Update()
    {
        if (didSetLeaderboardBtn == false && ClientData.UserProfile != null)
        {
            if (ClientData.UserProfile.IsAnonymous)
            {
                leaderBoardButton.gameObject.SetActive(false);
            }
            else
            {
                leaderBoardButton.gameObject.SetActive(true);
            }
            didSetLeaderboardBtn = true;
        }
    }
    /*
    public void OnLeaderboardButtonClicked()
    {
        if (ClientData.UserProfile.IsAnonymous)
        {
            return;
        }
        SceneManager.LoadScene(SceneNames.LeaderboardPage, LoadSceneMode.Single);
    }*/
}
