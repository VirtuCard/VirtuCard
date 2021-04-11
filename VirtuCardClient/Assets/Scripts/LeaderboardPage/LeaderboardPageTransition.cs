using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardPageTransition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnLeaderboardButtonClicked()
    {
        if (ClientData.UserProfile.IsAnonymous)
        {
            return;
        }
        SceneManager.LoadScene(SceneNames.LeaderboardPage, LoadSceneMode.Single);
    }
}
