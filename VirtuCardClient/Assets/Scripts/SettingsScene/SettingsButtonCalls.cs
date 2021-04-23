using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButtonCalls : MonoBehaviour
{
    private int successful = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (successful == 1)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        if (successful == 2)
        {
            SceneManager.LoadScene(SceneNames.LandingPage);
        }

        successful = 0;
    }

    public void OnExitGameButtonClick()
    {
        if (ClientData.UserProfile.IsAnonymous)
        {
            AuthUser.DeleteAnonymousAccount(task =>
            {
                Debug.Log("Completed with return value " + task);
                successful = 1;
            });
        }
        else
        {
            successful = 1;
        }
    }

    public void OnLogoutButtonClick()
    {
        if (ClientData.UserProfile.IsAnonymous)
        {
            ClientData.UserProfile = null;
            AuthUser.DeleteAnonymousAccount(task =>
            {
                Debug.Log("Completed account deletion with return value " + task);
                successful = 2;
            });
        }
        else
        {
            ClientData.UserProfile = null;
            AuthUser.Logout();
            successful = 2;
        }
    }
}